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
using kcp2k;
using LiteNetLib;

namespace NetworkBenchmark.Kcp2k
{
	internal class EchoClient
	{
		public bool IsConnected { get; private set; }
		public bool IsDisposed { get; private set; }

		private readonly int id;
		private readonly BenchmarkSetup config;
		private readonly BenchmarkData benchmarkData;

		private readonly Thread tickThread;
		private readonly byte[] messageArray;
		private readonly KcpClient client;
		private KcpChannel communicationChannel;

		public EchoClient(int id, BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.id = id;
			this.config = config;
			this.benchmarkData = benchmarkData;
			messageArray = config.Message;
			communicationChannel = KcpChannel.Unreliable;

			client = new KcpClient(OnPeerConnected, OnNetworkReceive, OnPeerDisconnected);

			tickThread = new Thread(Tick);
			tickThread.Name = $"Kcp2k Client {id}";
			tickThread.IsBackground = true;

			IsConnected = false;
			IsDisposed = false;
		}

		public void Start()
		{
			var interval = (uint) Utilities.CalculateTimeout(config.ClientTickRate);
			client.Connect(config.Address, (ushort)config.Port, true, interval);
			tickThread.Start();
			IsDisposed = false;
		}

		private void Tick()
		{
			while (benchmarkData.Listen)
			{
				client?.Tick();
				Thread.Sleep(0);
			}
		}

		public void StartSendingMessages()
		{
			var parallelMessagesPerClient = config.ParallelMessages;

			for (int i = 0; i < parallelMessagesPerClient; i++)
			{
				Send(messageArray, communicationChannel);
			}

			client.Tick();
		}

		public Task Disconnect()
		{
			if (!IsConnected)
			{
				return Task.CompletedTask;
			}

			var clientDisconnected = Task.Factory.StartNew(() => { client.Disconnect(); }, TaskCreationOptions.LongRunning);

			return clientDisconnected;
		}

		public async void Dispose()
		{
			while (tickThread.IsAlive)
			{
				await Task.Delay(10);
			}

			//TODO client.Dispose();
			IsDisposed = true;
		}

		private void Send(ArraySegment<byte> message, KcpChannel channel)
		{
			if (!IsConnected)
			{
				return;
			}

			client.Send(message, channel);
			Interlocked.Increment(ref benchmarkData.MessagesClientSent);
		}

		private void OnPeerConnected()
		{
			IsConnected = true;
		}

		private void OnNetworkReceive(ArraySegment<byte> arraySegment)
		{
			if (benchmarkData.Running)
			{
				Interlocked.Increment(ref benchmarkData.MessagesClientReceived);
				Send(messageArray, communicationChannel);
			}
		}

		private void OnPeerDisconnected()
		{
			IsConnected = false;
		}
	}
}
