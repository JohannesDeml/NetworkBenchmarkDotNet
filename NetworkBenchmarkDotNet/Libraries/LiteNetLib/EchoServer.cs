// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;

namespace NetworkBenchmark.LiteNetLib
{
	internal class EchoServer : AServer, INetEventListener
	{
		public override bool IsStarted => netManager != null && netManager.IsRunning;

		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly NetManager netManager;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;

			netManager = new NetManager(this);
			netManager.UpdateTime = Utilities.CalculateTimeout(config.ServerTickRate);
			netManager.UseNativeSockets = config.UseNativeSockets;
			if (!config.Address.Contains(':'))
			{
				netManager.IPv6Mode = IPv6Mode.Disabled;
			}

			netManager.UnsyncedEvents = true;
		}

		public override void StartServer()
		{
			base.StartServer();
			netManager.Start(config.Port);
		}

		public override void StopServer()
		{
			base.StopServer();
			netManager.Stop();
		}

		public override void Dispose() { }

		#region ManualMode

		public override void SendMessages(int messageCount, TransmissionType transmissionType)
		{
			var delivery = LiteNetLibBenchmark.GetDeliveryMethod(transmissionType);

			for (int i = 0; i < messageCount; i++)
			{
				Broadcast(MessageBuffer, delivery);
			}
			netManager.TriggerUpdate();
		}

		#endregion

		void INetEventListener.OnConnectionRequest(ConnectionRequest request)
		{
			if (netManager.ConnectedPeerList.Count > config.Clients)
			{
				Console.WriteLine("Too many clients try to connect to the server");
				request.Reject();
				return;
			}

			request.Accept();
		}
		private void Broadcast(byte[] data, DeliveryMethod delivery)
		{
			netManager.SendToAll(data, delivery);
			var messagesSent = netManager.ConnectedPeersCount;
			Interlocked.Add(ref benchmarkStatistics.MessagesServerSent, messagesSent);
		}

		void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
		{
			Interlocked.Increment(ref benchmarkStatistics.Errors);
		}

		void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
		{
			if (benchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesServerReceived);
				if (!ManualMode)
				{
					Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, MessageBuffer, 0, reader.UserDataSize);
					peer.Send(MessageBuffer, deliveryMethod);
					Interlocked.Increment(ref benchmarkStatistics.MessagesServerSent);
					netManager.TriggerUpdate();
				}
			}

			reader.Recycle();
		}

		void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
		void INetEventListener.OnPeerConnected(NetPeer peer) { }

		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
		{
			if (benchmarkPreparing || benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {peer.Id} disconnected while benchmark is running - {disconnectinfo.Reason}.");
			}
		}
	}
}
