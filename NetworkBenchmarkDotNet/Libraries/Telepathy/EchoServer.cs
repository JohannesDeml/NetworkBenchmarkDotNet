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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Telepathy;

namespace NetworkBenchmark.Telepathy
{
	internal class EchoServer : AServer
	{
		public override bool IsStarted => serverThread != null && serverThread.IsAlive && server.Active;

		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly Server server;
		private readonly Thread serverThread;
		private readonly bool noDelay;
		private readonly int interval;

		private readonly byte[] message;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			noDelay = true;
			interval = Utilities.CalculateTimeout(config.ServerTickRate);
			TelepathyBenchmark.ProcessTransmissionType(config.Transmission);


			server = new Server(512);
			server.NoDelay = noDelay;
			server.OnConnected = OnConnected;
			server.OnDisconnected = OnDisconnected;
			server.OnData = OnReceiveMessage;


			message = new byte[config.MessageByteSize];

			serverThread = new Thread(TickLoop);
			serverThread.Name = "Telepathy Server";
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
			var sw = new Stopwatch();

			while (listen)
			{
				sw.Restart();
				server.Tick(10000);

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
			// Checking for connection while running in client, since the server gets the connect after the client
		}

		private void OnReceiveMessage(int connectionId, ArraySegment<byte> arraySegment)
		{
			if (benchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesServerReceived);
				Array.Copy(arraySegment.Array, arraySegment.Offset, message, 0, arraySegment.Count);
				Send(connectionId, message);
			}
		}

		private void OnDisconnected(int connectionId)
		{
			if (benchmarkPreparing || benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {connectionId} disconnected while benchmark is running.");
			}
		}

		private void Send(int connectionId, ArraySegment<byte> message)
		{
			server.Send(connectionId, message);
			Interlocked.Increment(ref benchmarkStatistics.MessagesServerSent);
		}

		public override void Dispose()
		{
			// Server already stopped, maybe there is the need to dispose something else?
		}

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			TelepathyBenchmark.ProcessTransmissionType(transmissionType);

			for (int i = 0; i < messageCount; i++)
			{
				Broadcast(MessageBuffer);
			}
		}

		#endregion

		private void Broadcast(ArraySegment<byte> message)
		{
			// Annoying that this needs reflection, maybe there is a better way
			var fieldInfo = typeof(Server).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
			var clients = fieldInfo.GetValue(server) as ConcurrentDictionary<int, ConnectionState>;


			foreach (var connection in clients.Keys)
			{
				Send(connection, message);
			}

			var messagesSent = clients.Count;
			Interlocked.Add(ref benchmarkStatistics.MessagesServerSent, messagesSent);
		}
	}
}
