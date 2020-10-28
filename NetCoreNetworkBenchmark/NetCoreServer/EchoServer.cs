// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
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
using NetCoreServer;

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	class EchoServer: UdpServer
	{
		private readonly BenchmarkData benchmarkData;

		public EchoServer(BenchmarkConfiguration config, BenchmarkData benchmarkData): base(IPAddress.Any, config.Port)
		{
			this.benchmarkData = benchmarkData;
		}

		protected override void OnStarted()
		{
			// Start receive datagrams
			ReceiveAsync();
		}

		protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
		{
			// Continue receive datagrams.
			if (size == 0)
			{
				// Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}

			if (!benchmarkData.Running)
			{
				return;
			}

			benchmarkData.MessagesServerReceived++;
			// Echo the message back to the sender
			SendAsync(endpoint, buffer, offset, size);
		}

		protected override void OnSent(EndPoint endpoint, long sent)
		{
			// Continue receive datagrams.
			// Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
			ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });

			if (!benchmarkData.Running)
			{
				return;
			}

			benchmarkData.MessagesServerSent++;
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Server caught an error with code {error}");

			if (!benchmarkData.Running)
			{
				return;
			}

			benchmarkData.Errors++;
		}
	}
}
