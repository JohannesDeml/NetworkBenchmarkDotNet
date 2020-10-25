using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;

namespace NetCoreNetworkBenchmark.LiteNetLib
{
	internal class EchoServer
	{
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;
		private readonly Thread serverThread;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private readonly byte[] message;
		private readonly int tickRate;

		public EchoServer(BenchmarkConfiguration config)
		{
			this.config = config;
			benchmarkData = config.BenchmarkData;
			tickRate = Math.Max(1000 / this.config.TickRateServer, 1);

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);
			netManager.UpdateTime = tickRate;
			message = new byte[config.MessageByteSize];

			serverThread = new Thread(this.Start);
			serverThread.Name = "LiteNetLib Server";

			listener.ConnectionRequestEvent += OnConnectionRequest;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
		}

		public Task StartServerThread()
		{
			serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!serverThread.IsAlive)
				{
					Task.Delay(10);
				}
			});
			return serverStarted;
		}

		private void Start()
		{
			netManager.Start(config.Port);

			while (benchmarkData.Running)
			{
				netManager.PollEvents();
				Thread.Sleep(tickRate);
			}
		}

		public Task StopServerThread()
		{
			netManager.Stop();
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive)
				{
					Task.Delay(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			listener.ConnectionRequestEvent -= OnConnectionRequest;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;
		}

		private void OnConnectionRequest(ConnectionRequest request)
		{
			if (netManager.ConnectedPeerList.Count > config.NumClients)
			{
				Console.WriteLine("Too many clients try to connect to the server");
				request.Reject();
				return;
			}

			request.Accept();
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			Interlocked.Increment(ref benchmarkData.MessagesServerReceived);

			if (benchmarkData.Running)
			{
				Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, message, 0, reader.UserDataSize);
				peer.Send(message, deliverymethod);
				Interlocked.Increment(ref benchmarkData.MessagesServerSent);
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			Interlocked.Increment(ref benchmarkData.Errors);
		}
	}
}
