// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
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
using Telepathy;

namespace NetworkBenchmark.Telepathy
{
	internal class EchoClient : AClient
	{
		public override bool IsConnected => isConnected;
		public override bool IsStopped => tickThread == null || !tickThread.IsAlive;
		public override bool IsDisposed => isDisposed;

		private bool isConnected;
		private bool isDisposed;
		private readonly int id;
		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly Thread tickThread;
		private readonly byte[] messageArray;
		private readonly Client client;
		private readonly bool noDelay;
		private readonly int interval;

		public EchoClient(int id, Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.id = id;
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			messageArray = config.Message;
			noDelay = true;
			interval = Utilities.CalculateTimeout(config.ClientTickRate);
			TelepathyBenchmark.ProcessTransmissionType(config.Transmission);

			client = new Client(512);
			client.NoDelay = noDelay;

			client.OnConnected = OnPeerConnected;
			client.OnData = OnNetworkReceive;
			client.OnDisconnected = OnPeerDisconnected;

			tickThread = new Thread(TickLoop);
			tickThread.Name = $"Telepathy Client {id}";
			tickThread.IsBackground = true;

			isConnected = false;
			isDisposed = false;
		}

		public override void StartClient()
		{
			base.StartClient();
			tickThread.Start();
			isDisposed = false;
		}

		private void TickLoop()
		{
			client.Connect(config.Address, (ushort) config.Port);
			var sw = new Stopwatch();

			while (Listen)
			{
				sw.Restart();
				client.Tick(10000);

				var elapsed = sw.ElapsedMilliseconds;
				if (elapsed < interval)
				{
					TimeUtilities.HighPrecisionThreadSleep(interval - (int) elapsed);
				}
			}
		}

		public override void StartBenchmark()
		{
			base.StartBenchmark();
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(messageArray);
			}

			client.Tick(100);
		}

		public override void DisconnectClient()
		{
			if (!IsConnected)
			{
				return;
			}

			client.Disconnect();
		}

		public override void Dispose()
		{
			//TODO dispose client once supported
			isDisposed = true;
		}

		private void Send(ArraySegment<byte> message)
		{
			if (!IsConnected)
			{
				return;
			}

			client.Send(message);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}

		private void OnPeerConnected()
		{
			if (BenchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {id} connected while benchmark is running.");
			}

			isConnected = true;
		}

		private void OnNetworkReceive(ArraySegment<byte> arraySegment)
		{
			if (BenchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
				Send(messageArray);
			}
		}

		private void OnPeerDisconnected()
		{
			if (BenchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
			}

			isConnected = false;
		}
	}
}
