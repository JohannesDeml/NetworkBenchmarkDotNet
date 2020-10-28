// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnetClient.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal abstract class EnetClient
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

		public EnetClient(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;
			tickRate = Math.Max(1000 / this.config.TickRateClient, 1);

			host = new Host();
			address = new Address();
			address.SetHost(config.Address);
			address.Port = (ushort) config.Port;
			IsDisposed = false;
		}

		public abstract void Start();

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

		public virtual async void Dispose()
		{
			host.Flush();
			host.Dispose();
			IsDisposed = true;
		}

		protected void ConnectAndListen()
		{
			host.Create();
			peer = host.Connect(address, 4);

			while (benchmarkData.Listen)
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
						if (benchmarkData.Running)
						{
							Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
							OnReceiveMessage(netEvent);
						}
						netEvent.Packet.Dispose();

						break;
				}
			}
		}

		protected virtual void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(message);
			SendUnreliable(message, 0, peer);
		}

		protected void SendReliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.Reliable | PacketFlags.NoAllocate); // Reliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		protected void SendUnreliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}
	}
}
