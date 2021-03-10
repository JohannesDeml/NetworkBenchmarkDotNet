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

namespace NetworkBenchmark.LiteNetLib
{
	internal class EchoClient : AClient, IClient
	{
		public override bool IsConnected => isConnected;
		public override bool IsDisposed => isDisposed;

		private bool isConnected;
		private bool isDisposed;
		private readonly Configuration config;
		private readonly BenchmarkStatistics benchmarkStatistics;

		private readonly byte[] message;
		private readonly EventBasedNetListener listener;
		private readonly NetManager netManager;
		private readonly DeliveryMethod deliveryMethod;
		private NetPeer peer;

		public EchoClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics benchmarkStatistics) : base(id, clientGroup)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			message = config.Message;
			deliveryMethod = LiteNetLibBenchmark.GetDeliveryMethod(config.Transmission);

			listener = new EventBasedNetListener();
			netManager = new NetManager(listener);
			if (!config.Address.Contains(':'))
			{
				netManager.IPv6Enabled = IPv6Mode.Disabled;
			}

			netManager.UpdateTime = Utilities.CalculateTimeout(config.ClientTickRate);
			netManager.UnsyncedEvents = true;
			netManager.DisconnectTimeout = 10000;

			isConnected = false;
			isDisposed = false;

			listener.PeerConnectedEvent += OnPeerConnected;
			listener.PeerDisconnectedEvent += OnPeerDisconnected;
			listener.NetworkReceiveEvent += OnNetworkReceive;
			listener.NetworkErrorEvent += OnNetworkError;
		}

		public override void StartClient()
		{
			base.StartClient();
			netManager.Start();
			peer = netManager.Connect(config.Address, config.Port, "ConnectionKey");
			isDisposed = false;
		}

		public override void ConnectClient()
		{
			throw new NotImplementedException();
		}

		public override void Tick()
		{
			throw new NotImplementedException();
		}

		public override void StartBenchmark()
		{
			base.StartBenchmark();
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(message);
			}

			netManager.TriggerUpdate();
		}

		public override void DisconnectClient()
		{
			if (!IsConnected)
			{
				return;
			}

			// Run in own task to not block the main thread
			Task.Factory.StartNew(() => { peer.Disconnect(); }, TaskCreationOptions.LongRunning);
		}

		public override void StopClient()
		{
			base.StopClient();

			// Run in own task to not block the main thread
			Task.Factory.StartNew(() => { netManager.Stop(false); }, TaskCreationOptions.LongRunning);
		}

		public override void Dispose()
		{
			listener.PeerConnectedEvent -= OnPeerConnected;
			listener.PeerDisconnectedEvent -= OnPeerDisconnected;
			listener.NetworkReceiveEvent -= OnNetworkReceive;
			listener.NetworkErrorEvent -= OnNetworkError;

			isDisposed = true;
		}

		private void Send(byte[] bytes)
		{
			if (!IsConnected)
			{
				return;
			}

			peer.Send(bytes, deliveryMethod);
			Interlocked.Increment(ref benchmarkStatistics.MessagesClientSent);
		}

		private void OnPeerConnected(NetPeer peer)
		{
			isConnected = true;
		}

		private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			if (disconnectInfo.Reason == DisconnectReason.Timeout && (BenchmarkRunning || BenchmarkPreparing))
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected due to timeout. Probably the server is overwhelmed by the requests.");
				Interlocked.Increment(ref benchmarkStatistics.Errors);
			}

			this.peer = null;
			isConnected = false;
		}

		private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
		{
			if (BenchmarkRunning)
			{
				Interlocked.Increment(ref benchmarkStatistics.MessagesClientReceived);
				Send(message);
				netManager.TriggerUpdate();
			}

			reader.Recycle();
		}

		private void OnNetworkError(IPEndPoint endpoint, SocketError socketerror)
		{
			if (BenchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Error Client {id}: {socketerror}");
				Interlocked.Increment(ref benchmarkStatistics.Errors);
			}
		}
	}
}
