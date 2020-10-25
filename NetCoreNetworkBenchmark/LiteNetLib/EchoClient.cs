using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;

namespace NetCoreNetworkBenchmark.LiteNetLib
{
	internal class EchoClient
	{
		public bool IsConnected { get; private set; }
		public bool IsDisposed { get; private set; }

		private readonly int id;
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly int tickRate;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private NetPeer peer;
		private Task listenTask;

		public EchoClient(int id, BenchmarkConfiguration config)
		{
			this.id = id;
			this.config = config;
			benchmarkData = config.BenchmarkData;
			message = config.Message;
			tickRate = Math.Max(1000 / this.config.TickRateClient, 1);

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);

			IsConnected = false;
			IsDisposed = false;

			listener.PeerConnectedEvent += OnPeerConnected;
			listener.PeerDisconnectedEvent += OnPeerDisconnected;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
		}

		public void Start()
		{
			listenTask = Task.Factory.StartNew(ConnectAndListen, TaskCreationOptions.LongRunning);
			IsDisposed = false;
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessagesPerClient;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message, DeliveryMethod.Unreliable);
			}
		}

		public void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}

			if (peer == null)
			{
				Console.WriteLine($"Client {id} does not know peer even though it was connected, should not happen.");
				IsConnected = false;
				return;
			}

			// Disconnecting properly takes forever with 100+ clients
			Task.Factory.StartNew(() =>
			{
				peer.Disconnect();

				while (IsConnected)
				{
					netManager.PollEvents();
					Thread.Sleep(tickRate);
				}
			});
		}

		public void Stop()
		{
			// If not disconnected, stopping consumes a lot of time
			netManager.Stop(false);
		}

		public async void Dispose()
		{
			while (!listenTask.IsCompleted)
			{
				await Task.Delay(10);
			}

			listenTask.Dispose();

			listener.PeerConnectedEvent -= OnPeerConnected;
			listener.PeerDisconnectedEvent -= OnPeerDisconnected;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;

			IsDisposed = true;
		}

		private void ConnectAndListen()
		{
			netManager.Start();
			peer = netManager.Connect(config.Address, config.Port, "LiteNetLib");

			while (benchmarkData.Running || IsConnected)
			{
				netManager.PollEvents();
				Thread.Sleep(tickRate);
			}
		}

		private void Send(byte[] bytes, DeliveryMethod deliverymethod)
		{
			if (peer == null)
			{
				Interlocked.Increment(ref benchmarkData.Errors);
				if (netManager.FirstPeer == null)
				{
					Console.WriteLine($"Client {id} is missing the reference to the server");
					return;
				}

				peer = netManager.FirstPeer;
			}

			peer.Send(bytes, deliverymethod);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		private void OnPeerConnected(NetPeer peer)
		{
			IsConnected = true;
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			this.peer = null;
			IsConnected = false;
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
				Send(message, deliverymethod);
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.Errors);
			}
		}
	}
}
