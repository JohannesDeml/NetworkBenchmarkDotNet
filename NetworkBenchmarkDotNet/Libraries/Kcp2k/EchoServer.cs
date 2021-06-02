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
using System.Diagnostics;
using System.Threading;
using kcp2k;

namespace NetworkBenchmark.Kcp2k
{
	internal class EchoServer : AServer
	{
		public override bool IsStarted => serverThread != null && serverThread.IsAlive && server.IsActive();

		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly KcpServer server;
		private readonly Thread serverThread;
		private readonly KcpChannel communicationChannel;
		private readonly bool noDelay;
		private readonly int interval;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			noDelay = true;
			interval = Utilities.CalculateTimeout(config.ServerTickRate);
			communicationChannel = Kcp2kBenchmark.GetChannel(config.Transmission);


			server = new KcpServer(OnConnected, OnReceiveMessage, OnDisconnected, noDelay, (uint) interval);

			serverThread = new Thread(TickLoop);
			serverThread.Name = "Kcp2k Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public override void StartServer()
		{
			base.StartServer();
			serverThread.Start();
		}

		public override void Dispose()
		{
			// Server already stopped, maybe there is the need to dispose something else?
		}

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			var channel = Kcp2kBenchmark.GetChannel(transmissionType);

			for (int i = 0; i < messageCount; i++)
			{
				Broadcast(MessageBuffer, channel);
			}
		}

		#endregion

		private void TickLoop()
		{
			server.Start((ushort) config.Port);
			var sw = new Stopwatch();

			while (listen)
			{
				sw.Restart();
				server.Tick();

				var elapsed = sw.ElapsedMilliseconds;
				if (elapsed < interval)
				{
					TimeUtilities.HighPrecisionThreadSleep(interval - (int) elapsed);
				}
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
				if (!ManualMode)
				{
					Array.Copy(arraySegment.Array, arraySegment.Offset, MessageBuffer, 0, arraySegment.Count);
					Send(connectionId, MessageBuffer, communicationChannel);
				}
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

		private void Broadcast(ArraySegment<byte> message, KcpChannel channel)
		{
			foreach (var connection in server.connections.Values)
			{
				connection.SendData(message, channel);
			}

			var messagesSent = server.connections.Count;
			Interlocked.Add(ref benchmarkStatistics.MessagesServerSent, messagesSent);
		}
	}
}
