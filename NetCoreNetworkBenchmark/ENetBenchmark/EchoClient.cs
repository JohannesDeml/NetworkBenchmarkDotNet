using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoClient
	{
		private int _id;
		private byte[] _message;
		private int _initialMessages;
		private Host _host;
		private Address _address;
		private Peer _peer;
		public bool IsConnected => _peer.State == PeerState.Connected;
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
			_host = new Host();
			_address = new Address();
			_address.SetHost(config.Address);
			_address.Port = (ushort)config.Port;
		}

		public void Start()
		{
			_host.Create();
			_peer = _host.Connect(_address, 4);

			_listenTask = Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
		}

		public void StartSendingMessages()
		{
			for (int i = 0; i < _initialMessages; i++)
			{
				SendUnreliable(_message, 0, _peer);
			}
		}

		public void Disconnect()
		{
			_peer.Disconnect(0);
		}

		public void Dispose()
		{
			_listenTask.Dispose();
			_host.Flush();
			_host.Dispose();
		}

		private void Listen()
		{
			while (_benchmarkData.Running) {
				_host.Service(1000 / _config.TickRateClient, out Event netEvent);

				switch (netEvent.Type) {
					case EventType.None:
						break;

					case EventType.Connect:
						//Console.WriteLine($"Client {_id} connected!");
						break;

					case EventType.Receive:
						Interlocked.Increment(ref _benchmarkData.MessagesClientReceived);
						netEvent.Packet.CopyTo(_message);
						SendUnreliable(_message, 0, _peer);
						Interlocked.Increment(ref _benchmarkData.MessagesClientSent);

						netEvent.Packet.Dispose();

						break;
				}
			}
		}

		private void SendReliable(byte[] data, byte channelID, Peer peer) {
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.Reliable | PacketFlags.NoAllocate); // Reliable Sequenced
			peer.Send(channelID, ref packet);
		}

		private void SendUnreliable(byte[] data, byte channelID, Peer peer) {
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
		}
	}
}
