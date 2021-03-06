﻿// --------------------------------------------------------------------------------------------------------------------
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
	internal class EchoServer : AServer
	{
		public override bool IsStarted => netManager != null && netManager.IsRunning;

		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private readonly DeliveryMethod deliveryMethod;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics) : base(config)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			deliveryMethod = LiteNetLibBenchmark.GetDeliveryMethod(config.Transmission);

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);
			netManager.UpdateTime = Utilities.CalculateTimeout(config.ServerTickRate);
			if (!config.Address.Contains(':'))
			{
				// For LiteNetLib 1.0 and above
				//netManager.IPv6Mode = IPv6Mode.Disabled;

				// LiteNetLib up to 0.9.4
				netManager.IPv6Enabled = IPv6Mode.Disabled;
			}

			netManager.UnsyncedEvents = true;

			listener.ConnectionRequestEvent += OnConnectionRequest;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
			listener.PeerDisconnectedEvent += OnPeerDisconnected;
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

		public override void Dispose()
		{
			listener.ConnectionRequestEvent -= OnConnectionRequest;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;
			listener.PeerDisconnectedEvent -= OnPeerDisconnected;
		}

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

		private void OnConnectionRequest(ConnectionRequest request)
		{
			if (netManager.ConnectedPeerList.Count > config.Clients)
			{
				Console.WriteLine("Too many clients try to connect to the server");
				request.Reject();
				return;
			}

			request.Accept();
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod clientDeliveryMethod)
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

		private void Broadcast(byte[] data, DeliveryMethod delivery)
		{
			netManager.SendToAll(data, delivery);
			var messagesSent = netManager.ConnectedPeersCount;
			Interlocked.Add(ref benchmarkStatistics.MessagesServerSent, messagesSent);
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			Interlocked.Increment(ref benchmarkStatistics.Errors);
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
		{
			if (benchmarkPreparing || benchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {peer.Id} disconnected while benchmark is running - {disconnectinfo.Reason}.");
			}
		}
	}
}
