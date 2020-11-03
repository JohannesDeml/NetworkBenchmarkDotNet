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
		private readonly int timeout;
		private readonly Host host;
		private readonly Address address;
		private Peer peer;

		public EnetClient(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;
			timeout = Utilities.CalculateTimeout(this.config.ClientTickRate);

			host = new Host();
			address = new Address();
			address.SetHost(config.Address);
			address.Port = (ushort) config.Port;
			IsDisposed = false;
		}

		public abstract void Start();

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				SendUnreliable(message, 0, peer);
			}
		}

		public void Disconnect()
		{
			peer.DisconnectNow(0);
		}

		public virtual void Dispose()
		{
			host.Flush();
			host.Dispose();
			IsDisposed = true;
		}

		protected void ConnectAndListen()
		{
			host.Create();
			peer = host.Connect(address, 4);

			Event netEvent;

			while (benchmarkData.Listen)
			{
				bool polled = false;

				while (!polled)
				{
					if (host.CheckEvents(out netEvent) <= 0)
					{
						// blocks up to the timeout if no events are received
						// if a packet is received earlier, it stops blocking
						if (host.Service(timeout, out netEvent) <= 0)
							break;

						polled = true;
					}

					switch (netEvent.Type)
					{
						case EventType.None:
							break;

						case EventType.Disconnect:
							if (benchmarkData.Running)
							{
								Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
							}

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
