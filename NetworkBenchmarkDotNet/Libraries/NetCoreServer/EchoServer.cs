// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
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
		private volatile bool listen;
		private volatile bool benchmarkRunning;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly bool ManualMode;
		private readonly byte[] message;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(IPAddress.Parse(config.Address), config.Port)
		{
			ManualMode = config.Test == TestType.Manual;
			// Use Pinned Object Heap to reduce GC pressure
			message = GC.AllocateArray<byte>(config.MessageByteSize, true);
			config.Message.CopyTo(message, 0);

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

		#region ManualMode

		public void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			NetCoreServerBenchmark.ProcessTransmissionType(transmissionType);
			
			for (int i = 0; i < messageCount; i++)
			{
				Broadcast(message);
			}
		}

		#endregion

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
				if (!ManualMode)
				{
					// Echo the message back to the sender
					SendAsync(endpoint, buffer, offset, size);
					return;
				}
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

		private void Broadcast(byte[] bytes)
		{
			throw new NotImplementedException();
		}
	}
}
