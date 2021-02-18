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
		private readonly byte[] message;
		private readonly DeliveryMethod deliveryMethod;

		public EchoServer(Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			deliveryMethod = LiteNetLibBenchmark.GetDeliveryMethod(config.Transmission);

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
			Interlocked.Increment(ref benchmarkStatistics.MessagesServerReceived);

			if (benchmarkRunning)
			{
				Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, message, 0, reader.UserDataSize);
				peer.Send(message, deliveryMethod);
				Interlocked.Increment(ref benchmarkStatistics.MessagesServerSent);
				netManager.TriggerUpdate();
			}

			reader.Recycle();
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
