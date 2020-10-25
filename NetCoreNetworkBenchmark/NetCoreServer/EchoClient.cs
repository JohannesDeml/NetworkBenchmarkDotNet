using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UdpClient = NetCoreServer.UdpClient;

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	class EchoClient: UdpClient
	{
		private readonly byte[] _message;
		private readonly int _initialMessages;
		private readonly BenchmarkData _benchmarkData;

		public EchoClient(BenchmarkConfiguration config): base(config.Address, config.Port)
		{
			_message = config.Message;
			_initialMessages = config.ParallelMessagesPerClient;
			_benchmarkData = config.BenchmarkData;
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

			if (!_benchmarkData.Running)
			{
				return;
			}

			_benchmarkData.MessagesClientReceived++;
			SendMessage();
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Client caught an error with code {error}");

			if (!_benchmarkData.Running)
			{
				return;
			}

			_benchmarkData.Errors++;
		}

		public void StartSendingMessages()
		{
			for (int i = 0; i < _initialMessages; i++)
			{
				SendMessage();
			}
		}

		private void SendMessage()
		{
			Send(_message);
			_benchmarkData.MessagesClientSent++;
		}
	}
}
