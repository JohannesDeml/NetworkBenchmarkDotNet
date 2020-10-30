// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using ElfhildNet;

namespace NetCoreNetworkBenchmark.ElfhildNet
{
	internal class EchoClient
	{
		public ConnectionState State => connection?.State ?? ConnectionState.Disconnected;
		public bool IsDisposed { get; private set; }

		private readonly int id;
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly int tickRate;
		private readonly float deltaTickRate;
		private readonly NetManager netManager;
		private Connection connection;
		private Task listenTask;

		public EchoClient(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;
			tickRate = Math.Max(1000 / this.config.TickRateClient, 1);
			deltaTickRate = tickRate / 1000.0f;

			netManager = new NetManager();

			IsDisposed = false;
		}

		public void Start()
		{
			listenTask = Task.Factory.StartNew(ConnectAndListen, TaskCreationOptions.LongRunning);

			IsDisposed = false;
		}

		private void ConnectAndListen()
		{
			connection = netManager.Connect(config.Address, config.Port, "ConnectionKey");
			connection.PacketReceived += (ByteBuffer byteBuffer) => { OnNetworkReceive(connection, byteBuffer); };
			connection.Disconnected += OnDisconnect;

			while (benchmarkData.Listen)
			{
				netManager.Poll();
				netManager.Update(deltaTickRate);

				Thread.Sleep(tickRate);
			}
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessagesPerClient;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message);
			}

			netManager.Update(deltaTickRate);
		}

		public Task Disconnect()
		{
			if (State != ConnectionState.Connected)
			{
				return Task.CompletedTask;
			}

			connection.Disconnect();

			var clientDisconnected = Task.Run(() =>
			{
				while (State == ConnectionState.Connected)
				{
					Thread.Sleep(10);
				}
			});

			return clientDisconnected;
		}

		public Task Stop()
		{
			return listenTask;
		}

		public void Dispose()
		{
			IsDisposed = true;
		}

		private void Send(byte[] bytes)
		{
			if (State != ConnectionState.Connected)
			{
				return;
			}


			connection.BeginUnreliable();

			connection.Current.PutBytesWithLength(message, 0, message.Length);

			connection.EndUnreliable();


			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		private void OnDisconnect()
		{
			Utilities.WriteVerboseLine($"Client {id} disconnected.");
		}

		private void OnNetworkReceive(Connection connection, ByteBuffer byteBuffer)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
				Send(message);

				connection.Update(tickRate);
			}
		}
	}
}