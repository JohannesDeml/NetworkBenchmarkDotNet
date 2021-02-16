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
	internal class EchoClient : IThreadedClient
	{
		public bool IsConnected => peer.State == PeerState.Connected;
		public bool IsDisposed { get; private set; }
		public Thread ClientThread => connectAndListenThread;

		private readonly int id;
		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly int timeout;
		private readonly PacketFlags packetFlags;
		private readonly Host host;
		private readonly Address address;
		private readonly Thread connectAndListenThread;
		private Peer peer;

		public EchoClient(int id, BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;
			timeout = Utilities.CalculateTimeout(this.config.ClientTickRate);
			switch (config.TransmissionType)
			{
				case TransmissionType.Reliable:
					packetFlags = PacketFlags.Reliable;
					break;
				case TransmissionType.Unreliable:
					packetFlags = PacketFlags.None;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(config), $"Transmission Type {config.TransmissionType} not supported");
			}

			host = new Host();
			address = new Address();
			address.SetHost(config.Address);
			address.Port = (ushort) config.Port;
			IsDisposed = false;

			connectAndListenThread = new Thread(ConnectAndListen);
			connectAndListenThread.Name = $"ENet Client {id}";
			connectAndListenThread.IsBackground = true;
		}

		public void Start()
		{
			connectAndListenThread.Start();
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message, 0, peer);
			}
		}

		public void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}

			peer.DisconnectNow(0);
		}

		public void Dispose()
		{
			host.Flush();
			host.Dispose();
			IsDisposed = true;
		}

		private void ConnectAndListen()
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
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}
	}
}
