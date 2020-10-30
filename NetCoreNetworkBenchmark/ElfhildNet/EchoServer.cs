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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ElfhildNet;

namespace NetCoreNetworkBenchmark.ElfhildNet
{
	internal class EchoServer
	{
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;
		private readonly NetManager netManager;
		private readonly List<Connection> connections;
		private readonly byte[] message;
		private readonly int tickRate;

		public EchoServer(BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			tickRate = Math.Max(1000 / this.config.TickRateServer, 1);

			netManager = new NetManager();
			connections = new List<Connection>();

			message = new byte[config.MessageByteSize];

			netManager.ConnectionRequestEvent += OnConnectionRequest;
		}

		public Task StartServer()
		{
			Start();
			// TODO is there an event to get when the server is fully started?
			return Task.Delay(100);
		}

		private void Start()
		{
			netManager.Start(config.Port);
		}

		public Task StopServer()
		{
			// TODO missing a stop server function
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			netManager.ConnectionRequestEvent -= OnConnectionRequest;
		}

		private void OnConnectionRequest(Func<Connection> accept, Action reject, string token)
		{
			Utilities.WriteVerboseLine("Connection request received!");
			Connection connection = accept();
			connections.Add(connection);

			connection.PacketReceived += (ByteBuffer byteBuffer) =>
			{
				OnNetworkReceive(connection, byteBuffer);
			};

			connection.Disconnected += () =>
			{
				OnDisconnected(connection);
			};
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
				Buffer.BlockCopy(byteBuffer.data, byteBuffer.position, message, 0, byteBuffer.size);
				Interlocked.Increment(ref benchmarkData.MessagesServerSent);

				connection.BeginUnreliable();
				// TODO Can I expect to current buffer to be empty?
				connection.Current.PutBytesWithLength(message, 0, message.Length);
				connection.BeginUnreliable();

				// TODO What delta is needed here?
				netManager.Update(tickRate);
			}
		}
	}
}
