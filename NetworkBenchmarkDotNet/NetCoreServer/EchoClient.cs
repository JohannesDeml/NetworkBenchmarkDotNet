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

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	class EchoClient: UdpClient
	{
		private readonly byte[] message;
		private readonly int initialMessages;
		private readonly BenchmarkData benchmarkData;

		public EchoClient(BenchmarkConfiguration config, BenchmarkData benchmarkData): base(config.Address, config.Port)
		{
			message = config.Message;
			initialMessages = config.ParallelMessages;
			this.benchmarkData = benchmarkData;
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

			if (!benchmarkData.Running)
			{
				return;
			}

			benchmarkData.MessagesClientReceived++;
			SendMessage();
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Client caught an error with code {error}");

			if (!benchmarkData.Running)
			{
				return;
			}

			benchmarkData.Errors++;
		}

		public void StartSendingMessages()
		{
			for (int i = 0; i < initialMessages; i++)
			{
				SendMessage();
			}
		}

		private void SendMessage()
		{
			Send(message);
			benchmarkData.MessagesClientSent++;
		}
	}
}
