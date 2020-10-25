using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoClient
	{
		public bool IsConnected => _peer.State == PeerState.Connected;
		public bool IsDisposed { get; private set; }

		private int _id;
		private readonly BenchmarkConfiguration _config;
		private readonly BenchmarkData _benchmarkData;

		private readonly byte[] _message;
		private readonly int _tickRate;
		private readonly Host _host;
		private readonly Address _address;
		private Peer _peer;
		private Task _listenTask;

		public EchoClient(int id, BenchmarkConfiguration config)
		{
			_id = id;
			_config = config;
			_benchmarkData = config.BenchmarkData;
			_message = config.Message;
			_tickRate = Math.Max(1000 / _config.TickRateClient, 1);

			_host = new Host();
			_address = new Address();
			_address.SetHost(config.Address);
			_address.Port = (ushort) config.Port;
			IsDisposed = false;
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
				SendUnreliable(_message, 0, _peer);
			}
		}

		public void Disconnect()
		{
			_peer.DisconnectNow(0);
		}

		public async void Dispose()
		{
			while (!_listenTask.IsCompleted)
			{
				await Task.Delay(10);
			}

			_listenTask.Dispose();
			_host.Flush();
			_host.Dispose();
			IsDisposed = true;
		}

		private void ConnectAndListen()
		{
			_host.Create();
			_peer = _host.Connect(_address, 4);

			while (_benchmarkData.Running)
			{
				_host.Service(_tickRate, out Event netEvent);

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Connect:
						//Console.WriteLine($"Client {_id} connected!");
						break;

					case EventType.Receive:
						Interlocked.Increment(ref _benchmarkData.MessagesClientReceived);
						netEvent.Packet.CopyTo(_message);
						SendUnreliable(_message, 0, _peer);

						netEvent.Packet.Dispose();

						break;
				}
			}
		}

		private void SendReliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.Reliable | PacketFlags.NoAllocate); // Reliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref _benchmarkData.MessagesClientSent);
		}

		private void SendUnreliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref _benchmarkData.MessagesClientSent);
		}
	}
}
