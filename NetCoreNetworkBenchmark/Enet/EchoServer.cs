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
		private readonly int tickRate;

		public EchoServer(BenchmarkConfiguration config)
		{
			this.config = config;
			benchmarkData = config.BenchmarkData;
			tickRate = Math.Max(1000 / this.config.TickRateServer, 1);

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
					Task.Delay(10);
				}
			});
			return serverStarted;
		}

		private void Start()
		{
			host.Create(address, config.NumClients);

			while (benchmarkData.Running)
			{
				host.Service(tickRate, out Event netEvent);

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Receive:
						Interlocked.Increment(ref benchmarkData.MessagesServerReceived);

						if (benchmarkData.Running)
						{
							netEvent.Packet.CopyTo(message);
							SendUnreliable(message, 0, netEvent.Peer);
							Interlocked.Increment(ref benchmarkData.MessagesServerSent);
						}

						netEvent.Packet.Dispose();

						break;
				}
			}
		}

		public Task StopServerThread()
		{
			var serverStopped = Task.Run(() =>
			{
				while (serverThread.IsAlive)
				{
					Task.Delay(10);
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
		}

		private void SendUnreliable(byte[] data, byte channelID, Peer peer)
		{
			Packet packet = default(Packet);

			packet.Create(data, data.Length, PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced
			peer.Send(channelID, ref packet);
		}
	}
}
