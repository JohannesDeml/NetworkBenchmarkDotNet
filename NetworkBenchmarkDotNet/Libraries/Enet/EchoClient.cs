// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
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
	internal class EchoClient : AClient
	{
		public override bool IsConnected => peer.State == PeerState.Connected;
		public override bool IsStopped => listenThread == null || !listenThread.IsAlive;
		public override bool IsDisposed => isDisposed;

		private bool isDisposed;
		private readonly int id;
		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly int timeout;
		private readonly PacketFlags packetFlags;
		private readonly Host host;
		private readonly Address address;
		private readonly Thread listenThread;
		private Peer peer;

		public EchoClient(int id, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.id = id;
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
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
			if (!ManualMode)
			{
				SendMessages(config.ParallelMessages, config.Transmission);
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

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			var flags = ENetBenchmark.GetPacketFlags(config.Transmission);

			// Don't do this in a real-world application, ENet is not thread safe
			// send should only be called in the thread that also calls host.Service
			for (int i = 0; i < messageCount; i++)
			{
				Send(Message, 0, peer, flags);
			}
		}

		#endregion

		private void ListenLoop()
		{
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

				case EventType.Timeout:
					if (BenchmarkPreparing || BenchmarkRunning)
					{
						Utilities.WriteVerboseLine($"Client {id} timed out while benchmark is running.");
					}

					break;

				case EventType.Disconnect:
					if (BenchmarkPreparing || BenchmarkRunning)
					{
						Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
					}

					break;

				case EventType.Receive:
					if (BenchmarkRunning)
					{
						Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
						if (!ManualMode)
						{
							OnReceiveMessage(netEvent);
						}
					}

					netEvent.Packet.Dispose();
					break;
			}
		}

		private void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(Message);
			Send(Message, 0, peer, packetFlags);
		}

		private void Send(byte[] data, byte channelID, Peer peer, PacketFlags flags)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, flags);
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}
	}
}
