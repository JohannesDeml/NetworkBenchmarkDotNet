using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoClient
	{
		public bool IsConnected => peer.State == PeerState.Connected;
		public bool IsDisposed { get; private set; }

		private int id;
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly int tickRate;
		private readonly Host host;
		private readonly Address address;
		private Peer peer;
		private Task listenTask;

		public EchoClient(int id, BenchmarkConfiguration config)
		{
			this.id = id;
			this.config = config;
			benchmarkData = config.BenchmarkData;
			message = config.Message;
			tickRate = Math.Max(1000 / this.config.TickRateClient, 1);

			host = new Host();
			address = new Address();
			address.SetHost(config.Address);
			address.Port = (ushort) config.Port;
			IsDisposed = false;
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
				SendUnreliable(message, 0, peer);
			}
		}

		public void Disconnect()
		{
			peer.DisconnectNow(0);
		}

		public async void Dispose()
		{
			while (!listenTask.IsCompleted)
			{
				await Task.Delay(10);
			}

			listenTask.Dispose();
			host.Flush();
			host.Dispose();
			IsDisposed = true;
		}

		private void ConnectAndListen()
		{
			host.Create();
			peer = host.Connect(address, 4);

			while (benchmarkData.Running)
			{
				host.Service(tickRate, out Event netEvent);

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Connect:
						//Console.WriteLine($"Client {_id} connected!");
						break;

					case EventType.Receive:
						Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
						netEvent.Packet.CopyTo(message);
						SendUnreliable(message, 0, peer);

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
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		private void SendUnreliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}
	}
}
