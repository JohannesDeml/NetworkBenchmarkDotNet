// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using kcp2k;

namespace NetworkBenchmark.Kcp2k
{
	internal class EchoServer
	{
		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;
		private readonly KcpServer server;
		private readonly Thread serverThread;
		private KcpChannel communicationChannel;
		private bool noDelay;

		private readonly byte[] message;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			communicationChannel = KcpChannel.Unreliable;
			noDelay = true;

			var interval = (uint) Utilities.CalculateTimeout(config.ServerTickRate);
			server = new KcpServer(OnConnected, OnReceiveMessage, OnDisconnected, noDelay, interval);


			message = new byte[config.MessageByteSize];

			serverThread = new Thread(TickLoop);
			serverThread.Name = "Kcp2k Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public Task StartServerThread()
		{
			serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!serverThread.IsAlive || !server.IsActive())
				{
					Thread.Sleep(10);
				}
			});
			return serverStarted;
		}

		private void TickLoop()
		{
			server.Start((ushort) config.Port);

			while (benchmarkData.Listen)
			{
				server.Tick();
				Thread.Sleep(1);
			}

			server.Stop();
		}

		private void OnConnected(int connectionId)
		{
		}

		private void OnReceiveMessage(int connectionId, ArraySegment<byte> arraySegment)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesServerReceived);
				// TODO copy message
				Send(connectionId, arraySegment, communicationChannel);
			}
		}

		private void OnDisconnected(int connectionId)
		{
			if (benchmarkData.Preparing || benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Client {connectionId} disconnected while benchmark is running.");
			}
		}

		private void Send(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			server.Send(connectionId, message, channel);
			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}

		public Task StopServerThread()
		{
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive)
				{
					Thread.Sleep(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			// TODO server.Dispose();
		}
	}
}
