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
using kcp2k;

namespace NetworkBenchmark.Kcp2k
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
		private readonly KcpClientConnection client;
		private readonly KcpChannel communicationChannel;
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
			communicationChannel = Kcp2kBenchmark.GetChannel(config.Transmission);

			client = new KcpClientConnection();

			client.OnAuthenticated = OnPeerConnected;
			client.OnData = OnNetworkReceive;
			client.OnDisconnected = OnPeerDisconnected;

			tickThread = new Thread(TickLoop);
			tickThread.Name = $"Kcp2k Client {id}";
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
			client.Connect(config.Address, (ushort) config.Port, noDelay, (uint) interval);
			var sw = new Stopwatch();

			while (Listen)
			{
				sw.Restart();
				Tick();

				var elapsed = sw.ElapsedMilliseconds;
				if (elapsed < interval)
				{
					TimeUtilities.HighPrecisionThreadSleep(interval - (int) elapsed);
				}
			}
		}

		private void Tick()
		{
			client.RawReceive();
			client.Tick();
		}

		public override void StartBenchmark()
		{
			base.StartBenchmark();
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(messageArray, communicationChannel);
			}

			Tick();
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

		private void Send(ArraySegment<byte> message, KcpChannel channel)
		{
			if (!IsConnected)
			{
				return;
			}

			client.SendData(message, channel);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}

		private void OnPeerConnected()
		{
			isConnected = true;
		}

		private void OnNetworkReceive(ArraySegment<byte> arraySegment)
		{
			if (BenchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
				Send(messageArray, communicationChannel);
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
