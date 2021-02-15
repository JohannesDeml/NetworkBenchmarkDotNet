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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using kcp2k;

namespace NetworkBenchmark.Kcp2k
{
	internal class EchoServer
	{
		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;
		private readonly KcpServer server;
		private readonly Thread serverThread;
		private KcpChannel communicationChannel;
		private bool noDelay;

		#if WINDOWS
		/// Get higher precision for Thread.Sleep on Windows
		/// See https://web.archive.org/web/20051125042113/http://www.dotnet247.com/247reference/msgs/57/289291.aspx
		/// See https://docs.microsoft.com/en-us/windows/win32/api/timeapi/nf-timeapi-timebeginperiod
		[DllImport("winmm.dll")]
		internal static extern uint timeBeginPeriod(uint period);

		[DllImport("winmm.dll")]
		internal static extern uint timeEndPeriod(uint period);
		#endif

		private readonly byte[] message;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			communicationChannel = KcpChannel.Unreliable;
			noDelay = true;

			var interval = (uint) Utilities.CalculateTimeout(config.ServerTickRate);
			server = new KcpServer(OnConnected, OnReceiveMessage, OnDisconnected, noDelay, interval);


			message = new byte[config.MessageByteSize];

			serverThread = new Thread(TickLoop);
			serverThread.Name = "Kcp2k Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public Task StartServerThread()
		{
			serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!serverThread.IsAlive || !server.IsActive())
				{
					Thread.Sleep(10);
				}
			});
			return serverStarted;
		}

		private void TickLoop()
		{
			server.Start((ushort) config.Port);

			while (benchmarkData.Listen)
			{
				server.Tick();

				#if WINDOWS
				timeBeginPeriod(1);
				#endif
				
				Thread.Sleep(1);

				#if WINDOWS
				timeEndPeriod(1);
				#endif
			}

			server.Stop();
		}

		private void OnConnected(int connectionId)
		{
		}

		private void OnReceiveMessage(int connectionId, ArraySegment<byte> arraySegment)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesServerReceived);
				// TODO copy message
				Send(connectionId, arraySegment, communicationChannel);
			}
		}

		private void OnDisconnected(int connectionId)
		{
			if (benchmarkData.Preparing || benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Client {connectionId} disconnected while benchmark is running.");
			}
		}

		private void Send(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			server.Send(connectionId, message, channel);
			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}

		public Task StopServerThread()
		{
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive)
				{
					Thread.Sleep(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			// TODO server.Dispose();
		}
	}
}
