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
		private BenchmarkConfiguration _config;
		private BenchmarkData _benchmarkData;
		private Thread _serverThread;
		private EventBasedNetListener _listener;
		private NetManager _netManager;
		private byte[] _message;

		public EchoServer(BenchmarkConfiguration config)
		{
			_config = config;
			_benchmarkData = config.BenchmarkData;
			_listener = new EventBasedNetListener();
			_netManager = new NetManager(_listener);
			_message = new byte[config.MessageByteSize];

			_serverThread = new Thread(this.Start);
			_serverThread.Name = "LiteNetLib Server";

			_listener.ConnectionRequestEvent += OnConnectionRequest;
			_listener.NetworkReceiveEvent += OnNetworkReceive;
			_listener.NetworkErrorEvent += OnNetworkError;
		}

		public Task StartServerThread()
		{
			_serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!_serverThread.IsAlive)
				{
					Task.Delay(10);
				}
			});
			return serverStarted;
		}

		private void Start()
		{
			_netManager.Start(_config.Port);

			while (_benchmarkData.Running) {
				_netManager.PollEvents();
				Thread.Sleep(1000 / _config.TickRateServer);
			}
		}

		public Task StopServerThread()
		{
			_netManager.Stop();
			var serverStopped = Task.Run(() =>
			{
				while (_serverThread.IsAlive)
				{
					Task.Delay(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			_listener.ConnectionRequestEvent -= OnConnectionRequest;
			_listener.NetworkReceiveEvent -= OnNetworkReceive;
			_listener.NetworkErrorEvent -= OnNetworkError;
		}

		private void OnConnectionRequest(ConnectionRequest request)
		{
			if (_netManager.ConnectedPeerList.Count > _config.NumClients)
			{
				Console.WriteLine("Too many clients try to connect to the server");
				request.Reject();
				return;
			}

			request.Accept();
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			Interlocked.Increment(ref _benchmarkData.MessagesServerReceived);

			if (_benchmarkData.Running)
			{
				Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, _message, 0, reader.UserDataSize);
				peer.Send(_message, DeliveryMethod.Unreliable);
				Interlocked.Increment(ref _benchmarkData.MessagesServerSent);
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			Interlocked.Increment(ref _benchmarkData.Errors);
		}
	}
}
