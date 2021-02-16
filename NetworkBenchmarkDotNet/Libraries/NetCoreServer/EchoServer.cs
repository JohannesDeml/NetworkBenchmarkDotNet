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

namespace NetworkBenchmark.NetCoreServer
{
	internal class EchoServer : UdpServer, IServer
	{
		private volatile bool benchmarkPreparing;
		private volatile bool listen;
		private volatile bool benchmarkRunning;
		private readonly BenchmarkData benchmarkData;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData) : base(IPAddress.Parse(config.Address), config.Port)
		{
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

			this.benchmarkData = benchmarkData;
		}

		public void StartServer()
		{
			base.Start();
			listen = true;
			benchmarkPreparing = true;
		}

		public void StartBenchmark()
		{
			benchmarkPreparing = false;
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
				benchmarkData.MessagesServerReceived++;
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
				benchmarkData.MessagesServerSent++;
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
				benchmarkData.Errors++;
			}
		}
	}
}
