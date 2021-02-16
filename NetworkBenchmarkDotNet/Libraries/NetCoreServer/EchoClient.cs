// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
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

		public EchoClient(int id, BenchmarkSetup config, BenchmarkStatistics benchmarkStatistics) : base(config.Address, config.Port)
		{
			this.id = id;
			switch (config.Transmission)
			{
				case TransmissionType.Reliable:
					Console.WriteLine("NetCoreServer with UDP does not support reliable message delivery");
					break;
				case TransmissionType.Unreliable:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(config), $"Transmission Type {config.Transmission} not supported");
			}

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

		protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
		{
			// Continue receive datagrams
			// Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
			ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });

			if (!benchmarkRunning)
			{
				return;
			}

			benchmarkStatistics.MessagesClientReceived++;
			SendMessage();
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Client {id} caught an error with code {error}");

			if (!benchmarkRunning)
			{
				return;
			}

			benchmarkStatistics.Errors++;
		}
	}
}
