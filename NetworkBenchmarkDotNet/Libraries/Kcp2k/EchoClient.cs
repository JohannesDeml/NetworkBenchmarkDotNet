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
		private readonly KcpClientConnection client;
		private readonly KcpChannel communicationChannel;
		private readonly bool noDelay;

		public EchoClient(int id, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.id = id;
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			noDelay = true;
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
			var interval = (uint) Utilities.CalculateTimeout(config.ClientTickRate);
			client.Connect(config.Address, (ushort) config.Port, noDelay, interval);

			while (Listen)
			{
				Tick();
				Thread.Sleep(1);
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
			if (!ManualMode)
			{
				SendMessages(config.ParallelMessages, config.Transmission);
			}
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

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			var channel = Kcp2kBenchmark.GetChannel(transmissionType);

			for (int i = 0; i < messageCount; i++)
			{
				Send(Message, channel);
			}
			Tick();
		}

		#endregion

		private void Send(ArraySegment<byte> buffer, KcpChannel channel)
		{
			if (!IsConnected)
			{
				return;
			}

			client.SendData(buffer, channel);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}

		private void OnPeerConnected()
		{
			Console.WriteLine("Benchmark client connected");
			isConnected = true;
		}

		private void OnNetworkReceive(ArraySegment<byte> arraySegment)
		{
			if (BenchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
				if (!ManualMode)
				{
					Send(Message, communicationChannel);
				}
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
