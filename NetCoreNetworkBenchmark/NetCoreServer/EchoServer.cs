using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetCoreServer;

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	class EchoServer : UdpServer
	{
		private BenchmarkData _benchmarkData;

		public EchoServer(BenchmarkConfiguration config) : base(IPAddress.Any, config.Port)
		{
			_benchmarkData = config.BenchmarkData;
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

			if (!_benchmarkData.Running)
			{
				return;
			}

			_benchmarkData.MessagesServerReceived++;
			// Echo the message back to the sender
			SendAsync(endpoint, buffer, offset, size);
		}

		protected override void OnSent(EndPoint endpoint, long sent)
		{
			// Continue receive datagrams.
			// Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
			ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); } );

			if (!_benchmarkData.Running)
			{
				return;
			}
			_benchmarkData.MessagesServerSent++;
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Server caught an error with code {error}");

			if (!_benchmarkData.Running)
			{
				return;
			}

			_benchmarkData.Errors++;
		}
	}
}
