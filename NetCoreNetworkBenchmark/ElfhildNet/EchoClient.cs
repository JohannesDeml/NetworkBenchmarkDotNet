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
		public ConnectionState State => connection.State;
		public bool IsDisposed { get; private set; }

		private readonly int id;
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly int tickRate;
		private readonly NetManager netManager;
		private Connection connection;

		public EchoClient(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;
			tickRate = Math.Max(1000 / this.config.TickRateClient, 1);

			netManager = new NetManager();

			IsDisposed = false;
		}

		public void Start()
		{
			connection = netManager.Connect(config.Address, config.Port, "ConnectionKey");

			connection.PacketReceived += (ByteBuffer byteBuffer) => { OnNetworkReceive(connection, byteBuffer); };

			connection.Disconnected += OnDisconnect;
			IsDisposed = false;
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessagesPerClient;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message);
			}

			netManager.Update(0.1f);
		}

		public Task Disconnect()
		{
			if (State != ConnectionState.Connected)
			{
				return Task.CompletedTask;
			}

			connection.Disconnect();

			var clientDisconnected = Task.Run(async () =>
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
			// TODO stopping is missing
			return Task.CompletedTask;
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
				connection.BeginUnreliable();
				Send(message);
				connection.EndUnreliable();

				connection.Update(tickRate);
			}
		}
	}
}
