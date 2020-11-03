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
using System.Diagnostics;
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
			tickRate = Math.Max(1000 / this.config.ClientTickRate, 1);
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

			Stopwatch timer = Stopwatch.StartNew();

			ByteBuffer buffer = ByteBuffer.Allocate();


			while (benchmarkData.Listen)
			{
				netManager.Poll(buffer, tickRate);

				netManager.Update((float)timer.Elapsed.TotalSeconds);

				timer.Restart();
			}

			ByteBuffer.Deallocate(buffer);
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message);
			}

			netManager.Update(deltaTickRate);
		}

		public void Disconnect()
		{
			if (State != ConnectionState.Connected)
			{
				return;
			}

			connection.Disconnect();
		}

		public Task Stop()
		{
			return listenTask;
		}

		public void Dispose()
		{
			IsDisposed = true;
		}

		private void OnDisconnect()
		{
			if (benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
			}
		}

		private void OnNetworkReceive(Connection connection, ByteBuffer byteBuffer)
		{
			if (benchmarkData.Running)
			{
				while (byteBuffer.HasData)
				{
					Buffer.BlockCopy(byteBuffer.data, byteBuffer.position, message, 0, message.Length);

					byteBuffer.position += message.Length;
					Interlocked.Increment(ref benchmarkData.MessagesClientReceived);

					Send(message);
				}
			}
		}

		private void Send(byte[] bytes)
		{
			if (State != ConnectionState.Connected)
			{
				return;
			}

			connection.BeginUnreliable();
			connection.Current.PutBytes(message, 0, message.Length);
			connection.EndUnreliable();

			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}
	}
}
