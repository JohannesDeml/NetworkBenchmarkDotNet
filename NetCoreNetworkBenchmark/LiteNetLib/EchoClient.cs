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

		private int _id;
		private BenchmarkConfiguration _config;
		private BenchmarkData _benchmarkData;

		private byte[] _message;
		private int _tickRate;
		private EventBasedNetListener _listener;
		private NetManager _netManager;
		private NetPeer _peer;
		private Task _listenTask;

		public EchoClient(int id, BenchmarkConfiguration config)
		{
			_id = id;
			_config = config;
			_benchmarkData = config.BenchmarkData;
			_message = config.Message;
			_tickRate = Math.Max(1000 / _config.TickRateClient, 1);

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
			var parallelMessagesPerClient = _config.ParallelMessagesPerClient;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(_message, DeliveryMethod.Unreliable);
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

			// Disconnecting properly takes forever with 100+ clients
			// Task.Factory.StartNew(() =>
			// {
			// 	_peer.Disconnect();
			//
			// 	while (IsConnected)
			// 	{
			// 		_netManager.PollEvents();
			// 		Thread.Sleep(_tickRate);
			// 	}
			// });

			IsConnected = false;
		}

		public void Stop()
		{
			// If not disconnected, stopping consumes a lot of time
			// _netManager.Stop(false);
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
			_peer = _netManager.Connect(_config.Address, _config.Port, "LiteNetLib");

			while (_benchmarkData.Running || IsConnected) {
				_netManager.PollEvents();
				Thread.Sleep(_tickRate);
			}
		}

		private void Send(byte[] bytes, DeliveryMethod deliverymethod)
		{
			if (_peer == null)
			{
				Interlocked.Increment(ref _benchmarkData.Errors);
				if (_netManager.FirstPeer == null)
				{
					Console.WriteLine($"Client {_id} is missing the reference to the server");
					return;
				}

				_peer = _netManager.FirstPeer;
			}
			_peer.Send(bytes, deliverymethod);
			Interlocked.Increment(ref _benchmarkData.MessagesClientSent);
		}

		private void OnPeerConnected(NetPeer peer)
		{
			IsConnected = true;
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			_peer = null;
			IsConnected = false;
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			if (_benchmarkData.Running)
			{
				Interlocked.Increment(ref _benchmarkData.MessagesClientReceived);
				Send(_message, deliverymethod);
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			if (_benchmarkData.Running)
			{
				Interlocked.Increment(ref _benchmarkData.Errors);
			}
		}
	}
}
