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
using kcp2k;

namespace NetworkBenchmark.Kcp2k
{
	internal class EchoServer : AServer
	{
		public override bool IsStarted => serverThread != null && serverThread.IsAlive && server.IsActive();

		private readonly BenchmarkSetup config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly KcpServer server;
		private readonly Thread serverThread;
		private readonly KcpChannel communicationChannel;
		private readonly bool noDelay;

		private readonly byte[] message;

		public EchoServer(BenchmarkSetup config, BenchmarkStatistics benchmarkStatistics)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			noDelay = true;
			communicationChannel = Kcp2kBenchmark.GetChannel(config.Transmission);


			var interval = (uint) Utilities.CalculateTimeout(config.ServerTickRate);
			server = new KcpServer(OnConnected, OnReceiveMessage, OnDisconnected, noDelay, interval);


			message = new byte[config.MessageByteSize];

			serverThread = new Thread(TickLoop);
			serverThread.Name = "Kcp2k Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public override void StartServer()
		{
			base.StartServer();
			serverThread.Start();
		}

		private void TickLoop()
		{
			server.Start((ushort) config.Port);

			while (listen)
			{
				server.Tick();
				TimeUtilities.HighPrecisionThreadSleep(1);
			}

			server.Stop();
		}

		private void OnConnected(int connectionId)
		{
			if (benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {connectionId} connected while benchmark is running.");
			}
		}

		private void OnReceiveMessage(int connectionId, ArraySegment<byte> arraySegment)
		{
			if (benchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesServerReceived);
				Array.Copy(arraySegment.Array, arraySegment.Offset, message, 0, arraySegment.Count);
				Send(connectionId, message, communicationChannel);
			}
		}

		private void OnDisconnected(int connectionId)
		{
			if (benchmarkPreparing || benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {connectionId} disconnected while benchmark is running.");
			}
		}

		private void Send(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			server.Send(connectionId, message, channel);
			Interlocked.Increment(ref benchmarkStatistics.MessagesServerSent);
		}

		public override void Dispose()
		{
			// Server already stopped, maybe there is the need to dispose something else?
		}
	}
}
