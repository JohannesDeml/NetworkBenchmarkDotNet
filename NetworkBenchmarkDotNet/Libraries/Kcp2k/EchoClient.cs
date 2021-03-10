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
		public override bool IsDisposed => isDisposed;

		private bool isConnected;
		private bool isDisposed;
		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly byte[] messageArray;
		private readonly KcpClientConnection client;
		private readonly KcpChannel communicationChannel;
		private readonly bool noDelay;

		public EchoClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(id, clientGroup)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			messageArray = config.Message;
			noDelay = true;
			communicationChannel = Kcp2kBenchmark.GetChannel(config.Transmission);

			client = new KcpClientConnection();

			client.OnAuthenticated = OnConnected;
			client.OnData = OnNetworkReceive;
			client.OnDisconnected = OnDisconnected;

			isConnected = false;
			isDisposed = false;
		}

		public override void StartClient()
		{
			base.StartClient();
			isDisposed = false;
		}

		public override void ConnectClient()
		{
			var interval = (uint) Utilities.CalculateTimeout(config.ClientTickRate);
			client.Connect(config.Address, (ushort) config.Port, noDelay, interval);
		}

		public override void Tick(int elapsedMs)
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

			Tick(0);
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
			base.Dispose();
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

		protected override void OnConnected()
		{
			base.OnConnected();
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

		protected override void OnDisconnected()
		{
			base.OnDisconnected();
			isConnected = false;
		}
	}
}
