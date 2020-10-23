﻿using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoServer
	{
		private BenchmarkConfiguration _config;
		private BenchmarkData _benchmarkData;
		private Thread _serverThread;
		private Host _host;
		private Address _address;
		private byte[] _message;

		public EchoServer(BenchmarkConfiguration config)
		{
			_config = config;
			_benchmarkData = config.BenchmarkData;
			_host = new Host();
			_address = new Address();
			_message = new byte[config.MessageByteSize];

			_address.Port = (ushort)config.Port;
			_address.SetHost(config.Address);
			_serverThread = new Thread(this.Start);
			_serverThread.Name = "Enet Server";
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
			_host.Create(_address, _config.NumClients);

			while (_benchmarkData.Running) {
				_host.Service(1000 / _config.TickRateServer, out Event netEvent);

				switch (netEvent.Type) {
					case EventType.None:
						break;

					case EventType.Receive:
						Interlocked.Increment(ref _benchmarkData.MessagesServerReceived);

						if (_benchmarkData.Running)
						{
							netEvent.Packet.CopyTo(_message);
							SendUnreliable(_message, 0, netEvent.Peer);
							Interlocked.Increment(ref _benchmarkData.MessagesServerSent);
						}

						netEvent.Packet.Dispose();

						break;
				}
			}
		}

		public Task StopServerThread()
		{
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
			_host.Flush();
			_host.Dispose();
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