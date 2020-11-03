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

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoServer
	{
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;
		private readonly Thread serverThread;
		private readonly Host host;
		private readonly Address address;
		private readonly byte[] message;
		private readonly int timeout;

		public EchoServer(BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			timeout = Utilities.CalculateTimeout(this.config.ServerTickRate);

			host = new Host();
			address = new Address();
			message = new byte[config.MessageByteSize];

			address.Port = (ushort) config.Port;
			address.SetHost(config.Address);
			serverThread = new Thread(this.Start);
			serverThread.Name = "Enet Server";
		}

		public Task StartServerThread()
		{
			serverThread.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!serverThread.IsAlive)
				{
					Thread.Sleep(10);
				}
			});
			return serverStarted;
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
							if (benchmarkData.Running)
							{
								Utilities.WriteVerboseLine($"Client {netEvent.Peer.ID} disconnected while benchmark is running.");
							}

							break;
					}
				}
			}
		}

		private void OnReceiveMessage(Event netEvent)
		{
			netEvent.Packet.CopyTo(message);
			SendUnreliable(message, 0, netEvent.Peer);
		}

		public Task StopServerThread()
		{
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive)
				{
					Thread.Sleep(10);
				}
			});
			return serverStopped;
		}

		public void Dispose()
		{
			host.Flush();
			host.Dispose();
		}

		private void SendReliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.Reliable | PacketFlags.NoAllocate); // Reliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}

		private void SendUnreliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
			Interlocked.Increment(ref benchmarkData.MessagesServerSent);
		}
	}
}
