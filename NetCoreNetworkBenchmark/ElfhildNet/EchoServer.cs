// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElfhildNet;

namespace NetCoreNetworkBenchmark.ElfhildNet
{
	internal class EchoServer
	{
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;
		private readonly Thread serverThread;
		private readonly NetManager netManager;
		private readonly List<Connection> connections;
		private readonly byte[] message;
		private readonly int tickRate;
		private readonly float deltaTickRate;

		public EchoServer(BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			tickRate = Math.Max(1000 / this.config.TickRateServer, 1);
			deltaTickRate = tickRate / 1000.0f;

			netManager = new NetManager();
			connections = new List<Connection>();

			message = new byte[config.MessageByteSize];

			netManager.ConnectionRequestEvent += OnConnectionRequest;

			serverThread = new Thread(this.Start);
			serverThread.Name = "Elfhild Server";
		}

		public Task StartServerThread()
		{
			serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!serverThread.IsAlive)
				{
					Thread.Sleep(10);
				}
			});
			return serverStarted;
		}

		private void Start()
		{
			netManager.Start(config.Port);

			while (benchmarkData.Listen)
			{
				netManager.Poll();

				netManager.Update(deltaTickRate);

				Thread.Sleep(tickRate);
			}
		}

		public Task StopServerThread()
		{
			netManager.Stop();
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive || netManager.IsRunning)
				{
					Thread.Sleep(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			netManager.ConnectionRequestEvent -= OnConnectionRequest;
		}

		private void OnConnectionRequest(Func<Connection> accept, Action reject, string token)
		{
			Connection connection = accept();
			connections.Add(connection);

			connection.PacketReceived += (ByteBuffer byteBuffer) => { OnNetworkReceive(connection, byteBuffer); };

			connection.Disconnected += () => { OnDisconnected(connection); };
		}

		private void OnDisconnected(Connection connection)
		{
			connections.Remove(connection);
		}

		private void OnNetworkReceive(Connection connection, ByteBuffer byteBuffer)
		{
			Interlocked.Increment(ref benchmarkData.MessagesServerReceived);

			if (benchmarkData.Running)
			{
				while (byteBuffer.HasData)
				{
					Buffer.BlockCopy(byteBuffer.data, byteBuffer.position + ElfhildNetBenchmark.HeaderSize, message, 0, message.Length);

					byteBuffer.position += message.Length + ElfhildNetBenchmark.HeaderSize;
					Interlocked.Increment(ref benchmarkData.MessagesServerReceived);

					Send(connection, message);
				}
			}
		}

		private void Send(Connection connection, byte[] bytes)
		{
			connection.BeginUnreliable();
			connection.Current.PutBytesWithLength(message, 0, message.Length);
			connection.EndUnreliable();

			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}
	}
}
