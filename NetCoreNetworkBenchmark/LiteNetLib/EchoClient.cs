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
		private int _id;
		private byte[] _message;
		private int _initialMessages;
		private EventBasedNetListener _listener;
		private NetManager _netManager;
		private NetPeer _peer;
		public bool IsConnected { get; private set; }
		public bool IsDisposed { get; private set; }
		private Task _listenTask;
		private BenchmarkConfiguration _config;
		private BenchmarkData _benchmarkData;
		public EchoClient(int id, BenchmarkConfiguration config)
		{
			_id = id;
			_message = config.Message;
			_initialMessages = config.ParallelMessagesPerClient;
			_config = config;
			_benchmarkData = config.BenchmarkData;

			_listener = new EventBasedNetListener();
			_netManager = new NetManager(_listener);

			IsConnected = false;
			IsDisposed = false;

			_listener.PeerConnectedEvent += OnPeerConnected;
			_listener.PeerDisconnectedEvent += OnPeerDisconnected;
			_listener.NetworkReceiveEvent += OnNetworkReceive;
			_listener.NetworkErrorEvent += OnNetworkError;
		}

		public void Start()
		{
			_listenTask = Task.Factory.StartNew(ConnectAndListen, TaskCreationOptions.LongRunning);
			IsDisposed = false;
		}

		public void StartSendingMessages()
		{
			for (int i = 0; i < _initialMessages; i++)
			{
				SendUnreliable(_message);
			}
		}

		public void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}

			if (_peer == null)
			{
				Console.WriteLine($"Client {_id} does not know peer even though it was connected, should not happen.");
				IsConnected = false;
				return;
			}

			_peer.Disconnect();
		}

		public async void Dispose()
		{
			while (!_listenTask.IsCompleted)
			{
				await Task.Delay(10);
			}
			_listenTask.Dispose();

			_listener.PeerConnectedEvent -= OnPeerConnected;
			_listener.PeerDisconnectedEvent -= OnPeerDisconnected;
			_listener.NetworkReceiveEvent -= OnNetworkReceive;
			_listener.NetworkErrorEvent -= OnNetworkError;
			IsDisposed = true;
		}

		private void ConnectAndListen()
		{
			_netManager.Start();
			_netManager.Connect(_config.Address, _config.Port, "LiteNetLib");

			while (_benchmarkData.Running) {
				_netManager.PollEvents();
				Thread.Sleep(1000 / _config.TickRateClient);
			}
		}

		private void SendUnreliable(byte[] bytes)
		{
			_peer.Send(bytes, DeliveryMethod.Unreliable);
			Interlocked.Increment(ref _benchmarkData.MessagesClientSent);
		}

		private void OnPeerConnected(NetPeer peer)
		{
			_peer = peer;
			IsConnected = true;
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			_peer = null;
			IsConnected = false;
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			Interlocked.Increment(ref _benchmarkData.MessagesClientReceived);

			if (_benchmarkData.Running)
			{
				SendUnreliable(_message);
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			Interlocked.Increment(ref _benchmarkData.Errors);
		}
	}
}
