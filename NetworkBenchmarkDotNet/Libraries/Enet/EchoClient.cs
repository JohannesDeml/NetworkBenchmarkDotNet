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
		public override bool IsStopped => listenThread == null || !listenThread.IsAlive;
		public override bool IsDisposed => isDisposed;

		private bool isDisposed;
		private readonly int id;
		private readonly BenchmarkSetup config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly byte[] message;
		private readonly int timeout;
		private readonly PacketFlags packetFlags;
		private readonly Host host;
		private readonly Address address;
		private readonly Thread listenThread;
		private Peer peer;

		public EchoClient(int id, BenchmarkSetup config, BenchmarkStatistics benchmarkStatistics)
		{
			this.id = id;
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

			listenThread = new Thread(ListenLoop);
			listenThread.Name = $"ENet Client {id}";
			listenThread.IsBackground = true;
		}

		public override void StartClient()
		{
			base.StartClient();
			listenThread.Start();
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

			peer.DisconnectNow(0);
		}

		public override void Dispose()
		{
			host.Flush();
			host.Dispose();
			isDisposed = true;
		}

		private void ListenLoop()
		{
			Listen = true;
			host.Create();
			peer = host.Connect(address, 4);

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
						if (host.Service(timeout, out netEvent) <= 0)
							break;

						polled = true;
					}

					HandleNetEvent(netEvent);
				}
			}
		}

		private void HandleNetEvent(Event netEvent)
		{
			switch (netEvent.Type)
			{
				case EventType.None:
					break;

				case EventType.Disconnect:
					if (BenchmarkRunning)
					{
						Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
					}

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
