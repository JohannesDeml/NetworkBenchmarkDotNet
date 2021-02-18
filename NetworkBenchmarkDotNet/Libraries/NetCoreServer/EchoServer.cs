// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetCoreServer;

namespace NetworkBenchmark.NetCoreServer
{
	internal class EchoServer : UdpServer, IServer
	{
		private volatile bool listen;
		private volatile bool benchmarkRunning;
		private readonly BenchmarkStatistics benchmarkStatistics;

		public EchoServer(BenchmarkSetup config, BenchmarkStatistics benchmarkStatistics) : base(IPAddress.Parse(config.Address), config.Port)
		{
			NetCoreServerBenchmark.ProcessTransmissionType(config.Transmission);
			this.benchmarkStatistics = benchmarkStatistics;
		}

		public void StartServer()
		{
			base.Start();
			listen = true;
		}

		public void StartBenchmark()
		{
			benchmarkRunning = true;
		}

		public void StopBenchmark()
		{
			benchmarkRunning = false;
		}

		public void StopServer()
		{
			base.Stop();
			listen = true;
		}

		protected override void OnStarted()
		{
			// Start receive datagrams
			ReceiveAsync();
		}

		protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
		{
			if (benchmarkRunning)
			{
				benchmarkStatistics.MessagesServerReceived++;
				// Echo the message back to the sender
				SendAsync(endpoint, buffer, offset, size);
				return;
			}

			// Keep listening for next possible benchmark
			if (listen)
			{
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}
		}

		protected override void OnSent(EndPoint endpoint, long sent)
		{
			if (benchmarkRunning)
			{
				benchmarkStatistics.MessagesServerSent++;
			}

			if (listen)
			{
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}
		}

		protected override void OnError(SocketError error)
		{
			if (benchmarkRunning)
			{
				benchmarkStatistics.Errors++;
			}
		}
	}
}
