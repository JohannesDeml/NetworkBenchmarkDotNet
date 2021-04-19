// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;
using System.Threading;
using UdpClient = NetCoreServer.UdpClient;

namespace NetworkBenchmark.NetCoreServer
{
	internal class EchoClient : UdpClient, IClient
	{
		public bool IsStopped => !IsConnected;

		private volatile bool benchmarkPreparing;
		private volatile bool listen;
		private volatile bool benchmarkRunning;

		private readonly int id;
		private readonly byte[] message;
		private readonly int initialMessages;
		private readonly BenchmarkStatistics benchmarkStatistics;

		public EchoClient(int id, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config.Address, config.Port)
		{
			this.id = id;
			NetCoreServerBenchmark.ProcessTransmissionType(config.Transmission);

			message = config.Message;
			initialMessages = config.ParallelMessages;
			this.benchmarkStatistics = benchmarkStatistics;
		}

		public void StartClient()
		{
			listen = true;
			benchmarkPreparing = true;
			Connect();
		}

		public void StartBenchmark()
		{
			benchmarkPreparing = false;
			benchmarkRunning = true;

			for (int i = 0; i < initialMessages; i++)
			{
				SendMessage();
			}
		}

		private void SendMessage()
		{
			Send(message);
			benchmarkStatistics.MessagesClientSent++;
		}

		public void StopBenchmark()
		{
			benchmarkRunning = false;
		}

		public void StopClient()
		{
			listen = false;
		}

		public void DisconnectClient()
		{
			Disconnect();
		}

		protected override void OnConnected()
		{
			// Start receive datagrams
			ReceiveAsync();
		}

		protected override void OnDisconnected()
		{
			base.OnDisconnected();

			if (benchmarkRunning || benchmarkPreparing)
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected due to timeout. Probably the server is overwhelmed by the requests.");
				Interlocked.Increment(ref benchmarkStatistics.Errors);
			}
		}

		protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
		{
			if (benchmarkRunning)
			{
				benchmarkStatistics.MessagesClientReceived++;
				SendMessage();
			}

			if (listen)
			{
				// Continue receive datagrams
				// Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}
		}

		protected override void OnError(SocketError error)
		{
			if (benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Error Client {id}: {error}");
				Interlocked.Increment(ref benchmarkStatistics.Errors);
			}
		}
	}
}
