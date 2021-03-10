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

namespace NetworkBenchmark.Enet
{
	internal class EchoClient : AClient
	{
		public override bool IsConnected => peer.State == PeerState.Connected;
		public override bool IsDisposed => isDisposed;

		private bool isDisposed;
		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly byte[] message;
		private readonly int timeout;
		private readonly PacketFlags packetFlags;
		private readonly Host host;
		private readonly Address address;
		private Peer peer;

		public EchoClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(id, clientGroup)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			message = config.Message;
			timeout = Utilities.CalculateTimeout(this.config.ClientTickRate);
			packetFlags = ENetBenchmark.GetPacketFlags(config.Transmission);

			host = new Host();
			address = new Address();
			address.SetHost(config.Address);
			address.Port = (ushort) config.Port;
			isDisposed = false;
		}

		public override void ConnectClient()
		{
			host.Create();
			peer = host.Connect(address, 4);
		}

		public override void Tick()
		{
			Event netEvent;

			while (Listen)
			{
				bool polled = false;

				while (!polled)
				{
					if (host.CheckEvents(out netEvent) <= 0)
					{
						// blocks up to the timeout if no events are received
						// if a packet is received earlier, it stops blocking
						if (host.Service(0, out netEvent) <= 0)
							return;

						polled = true;
					}

					HandleNetEvent(netEvent);
				}
			}
		}

		public override void StartBenchmark()
		{
			base.StartBenchmark();
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message, 0, peer);
			}
		}

		public override void DisconnectClient()
		{
			if (!IsConnected)
			{
				return;
			}

			peer.Disconnect(0);
		}

		public override void Dispose()
		{
			host.Flush();
			host.Dispose();
			isDisposed = true;
			base.Dispose();
		}

		private void HandleNetEvent(Event netEvent)
		{
			switch (netEvent.Type)
			{
				case EventType.None:
					break;

				case EventType.Timeout:
					if (BenchmarkPreparing || BenchmarkRunning)
					{
						Utilities.WriteVerboseLine($"Client {id} timed out while benchmark is running.");
					}
					break;

				case EventType.Connect:
					OnConnected();
					break;

				case EventType.Disconnect:
					OnDisconnected();
					break;

				case EventType.Receive:
					if (BenchmarkRunning)
					{
						Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
						OnReceiveMessage(netEvent);
					}

					netEvent.Packet.Dispose();
					break;
			}
		}

		private void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(message);
			Send(message, 0, peer);
		}

		private void Send(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, packetFlags);
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}
	}
}
