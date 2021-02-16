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
	class EchoServer : UdpServer
	{
		private readonly BenchmarkData benchmarkData;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData) : base(IPAddress.Parse(config.Address), config.Port)
		{
			switch (config.TransmissionType)
			{
				case TransmissionType.Reliable:
					Console.WriteLine("NetCoreServer with UDP does not support reliable message delivery");
					break;
				case TransmissionType.Unreliable:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(config), $"Transmission Type {config.TransmissionType} not supported");
			}
			
			this.benchmarkData = benchmarkData;
		}

		protected override void OnStarted()
		{
			// Start receive datagrams
			ReceiveAsync();
		}

		protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
		{
			if (benchmarkData.Running)
			{
				benchmarkData.MessagesServerReceived++;
				// Echo the message back to the sender
				SendAsync(endpoint, buffer, offset, size);
				return;
			}

			// Keep listening for next possible benchmark
			if (benchmarkData.Listen)
			{
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}
		}

		protected override void OnSent(EndPoint endpoint, long sent)
		{
			if (benchmarkData.Running)
			{
				benchmarkData.MessagesServerSent++;
			}

			if (benchmarkData.Listen)
			{
				ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
			}
		}

		protected override void OnError(SocketError error)
		{
			if (benchmarkData.Running)
			{
				benchmarkData.Errors++;
			}
		}
	}
}
