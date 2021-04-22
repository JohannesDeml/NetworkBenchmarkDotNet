// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using ENet;

namespace NetworkBenchmark.Enet
{
	internal class EchoServer : AServer
	{
		public override bool IsStarted => serverThread != null && serverThread.IsAlive;

		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly Thread serverThread;
		private readonly Host host;
		private readonly Address address;
		private readonly int timeout;
		private readonly PacketFlags packetFlags;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			timeout = Utilities.CalculateTimeout(this.config.ServerTickRate);
			packetFlags = ENetBenchmark.GetPacketFlags(config.Transmission);

			host = new Host();
			address = new Address();

			address.Port = (ushort) config.Port;
			address.SetHost(config.Address);
			serverThread = new Thread(ListenLoop);
			serverThread.Name = "Enet Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public override void StartServer()
		{
			base.StartServer();
			serverThread.Start();
		}

		private void ListenLoop()
		{
			host.Create(address, config.Clients);
			Event netEvent;

			while (listen)
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

				case EventType.Receive:
					if (benchmarkRunning)
					{
						Interlocked.Increment(ref benchmarkStatistics.MessagesServerReceived);
						if (!ManualMode)
						{
							OnReceiveMessage(netEvent);
						}
					}

					netEvent.Packet.Dispose();
					break;

				case EventType.Timeout:
					if (benchmarkPreparing || benchmarkRunning)
					{
						Utilities.WriteVerboseLine($"Client {netEvent.Peer.ID} disconnected while benchmark is running.");
					}

					break;
			}
		}

		private void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(MessageBuffer);
			Send(MessageBuffer, 0, netEvent.Peer);
		}

		public override void Dispose()
		{
			host.Flush();
			host.Dispose();
		}

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			// Don't do this in a real-world application, ENet is not thread safe
			// send should only be called in the thread that also calls host.Service
			for (int i = 0; i < messageCount; i++)
			{
				Broadcast(MessageBuffer, 0, transmissionType);
			}
		}

		#endregion

		private void Send(byte[] data, byte channelId, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, packetFlags);
			peer.Send(channelId, ref packet);
			Interlocked.Increment(ref benchmarkStatistics.MessagesServerSent);
		}

		private void Broadcast(byte[] data, byte channelId, TransmissionType transmissionType)
		{
			Packet packet = default(Packet);
			var flags = ENetBenchmark.GetPacketFlags(config.Transmission);

			packet.Create(data, data.Length, flags);
			host.Broadcast(channelId, ref packet);
			var messagesSent = host.PeersCount;
			Interlocked.Add(ref benchmarkStatistics.MessagesServerSent, messagesSent);
		}
	}
}
