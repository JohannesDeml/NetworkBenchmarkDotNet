// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetworkBenchmark.Enet
{
	internal class EchoServer : IServer
	{
		public bool IsStarted => serverThread != null && serverThread.IsAlive;

		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;
		private readonly Thread serverThread;
		private readonly Host host;
		private readonly Address address;
		private readonly byte[] message;
		private readonly int timeout;
		private readonly PacketFlags packetFlags;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			timeout = Utilities.CalculateTimeout(this.config.ServerTickRate);

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
			message = new byte[config.MessageByteSize];

			address.Port = (ushort) config.Port;
			address.SetHost(config.Address);
			serverThread = new Thread(this.Start);
			serverThread.Name = "Enet Server";
			serverThread.Priority = ThreadPriority.AboveNormal;
		}

		public void StartServerThread()
		{
			serverThread.Start();
		}

		private void Start()
		{
			host.Create(address, config.Clients);
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

				case EventType.Receive:
					if (benchmarkData.Running)
					{
						Interlocked.Increment(ref benchmarkData.MessagesServerReceived);
						OnReceiveMessage(netEvent);
					}

					netEvent.Packet.Dispose();
					break;

				case EventType.Timeout:
					if (benchmarkData.Preparing || benchmarkData.Running)
					{
						Utilities.WriteVerboseLine($"Client {netEvent.Peer.ID} disconnected while benchmark is running.");
					}

					break;
			}
		}

		private void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(message);
			Send(message, 0, netEvent.Peer);
		}

		public void Dispose()
		{
			host.Flush();
			host.Dispose();
		}

		private void Send(byte[] data, byte channelId, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, packetFlags);
			peer.Send(channelId, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}
	}
}
