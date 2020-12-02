// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClient.cs">
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
using System.Threading.Tasks;
using LiteNetLib;

namespace NetCoreNetworkBenchmark.LiteNetLib
{
	internal class EchoClient
	{
		public bool IsConnected { get; private set; }
		public bool IsDisposed { get; private set; }

		private readonly int id;
		private readonly BenchmarkConfiguration config;
		private readonly BenchmarkData benchmarkData;

		private readonly byte[] message;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private NetPeer peer;

		public EchoClient(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			message = config.Message;

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);
			netManager.IPv6Enabled = IPv6Mode.Disabled;
			netManager.UpdateTime = Utilities.CalculateTimeout(config.ClientTickRate);
			netManager.UnsyncedEvents = true;
			netManager.DisconnectTimeout = 10000;

			IsConnected = false;
			IsDisposed = false;

			listener.PeerConnectedEvent += OnPeerConnected;
			listener.PeerDisconnectedEvent += OnPeerDisconnected;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
		}

		public void Start()
		{
			netManager.Start();
			peer = netManager.Connect(config.Address, config.Port, "ConnectionKey");
			IsDisposed = false;
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message, DeliveryMethod.ReliableUnordered);
			}

			netManager.TriggerUpdate();
		}

		public Task Disconnect()
		{
			if (!IsConnected)
			{
				return Task.CompletedTask;
			}

			var clientDisconnected = Task.Factory.StartNew(() => { peer.Disconnect(); }, TaskCreationOptions.LongRunning);

			return clientDisconnected;
		}

		public Task Stop()
		{
			var stopClient = Task.Factory.StartNew(() => { netManager.Stop(false); }, TaskCreationOptions.LongRunning);

			return stopClient;
		}

		public void Dispose()
		{
			listener.PeerConnectedEvent -= OnPeerConnected;
			listener.PeerDisconnectedEvent -= OnPeerDisconnected;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;

			IsDisposed = true;
		}

		private void Send(byte[] bytes, DeliveryMethod deliverymethod)
		{
			if (!IsConnected)
			{
				return;
			}

			peer.Send(bytes, deliverymethod);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		private void OnPeerConnected(NetPeer peer)
		{
			IsConnected = true;
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			if (disconnectInfo.Reason == DisconnectReason.Timeout && benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected due to timeout. Probably the server is overwhelmed by the requests.");
				Interlocked.Increment(ref benchmarkData.Errors);
			}

			this.peer = null;
			IsConnected = false;
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
				Send(message, deliverymethod);
				netManager.TriggerUpdate();
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			if (benchmarkData.Running)
			{
				Utilities.WriteVerboseLine($"Error Client {id}: {socketerror}");
				Interlocked.Increment(ref benchmarkData.Errors);
			}
		}
	}
}
