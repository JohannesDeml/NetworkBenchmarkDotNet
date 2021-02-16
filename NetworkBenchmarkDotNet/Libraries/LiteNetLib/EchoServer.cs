﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoServer.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
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
	internal class EchoServer : IServer
	{
		public bool IsStarted => netManager != null && netManager.IsRunning;

		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private readonly byte[] message;
		private readonly DeliveryMethod deliveryMethod;

		public EchoServer(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;

			switch (config.TransmissionType)
			{
				case TransmissionType.Reliable:
					deliveryMethod = DeliveryMethod.ReliableUnordered;
					break;
				case TransmissionType.Unreliable:
					deliveryMethod = DeliveryMethod.ReliableUnordered;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(config), $"Transmission Type {config.TransmissionType} not supported");
			}

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);
			netManager.UpdateTime = Utilities.CalculateTimeout(config.ServerTickRate);
			if (!config.Address.Contains(':'))
			{
				netManager.IPv6Enabled = IPv6Mode.Disabled;
			}

			netManager.UnsyncedEvents = true;

			message = new byte[config.MessageByteSize];

			listener.ConnectionRequestEvent += OnConnectionRequest;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
			listener.PeerDisconnectedEvent += OnPeerDisconnected;
		}

		public void StartServer()
		{
			Start();
		}

		private void Start()
		{
			netManager.Start(config.Port);
		}

		public void StopServer()
		{
			netManager.Stop();
		}

		public void Dispose()
		{
			listener.ConnectionRequestEvent -= OnConnectionRequest;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;
			listener.PeerDisconnectedEvent -= OnPeerDisconnected;
		}

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
			Interlocked.Increment(ref benchmarkData.MessagesServerReceived);

			if (benchmarkData.Running)
			{
				Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, message, 0, reader.UserDataSize);
				peer.Send(message, deliveryMethod);
				Interlocked.Increment(ref benchmarkData.MessagesServerSent);
				netManager.TriggerUpdate();
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			Interlocked.Increment(ref benchmarkData.Errors);
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
		{
			if (benchmarkData.Preparing || benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Client {peer.Id} disconnected while benchmark is running - {disconnectinfo.Reason}.");
			}
		}
	}
}