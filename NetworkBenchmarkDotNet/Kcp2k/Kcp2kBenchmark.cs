﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kcp2kBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkBenchmark.Kcp2k
{
	internal class Kcp2kBenchmark : INetworkBenchmark
	{
		private BenchmarkSetup config;
		private BenchmarkData benchmarkData;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;

		public void Initialize(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			echoServer = new EchoServer(config, benchmarkData);
			echoClients = new List<EchoClient>();
		}

		public Task StartServer()
		{
			return echoServer.StartServerThread();
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients.Add(new EchoClient(i, config, benchmarkData));
			}

			return Task.CompletedTask;
		}

		public Task ConnectClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients[i].Start();
			}

			var clientsConnected = Task.Run(() =>
			{
				for (int i = 0; i < config.Clients; i++)
				{
					while (!echoClients[i].IsConnected)
					{
						Thread.Sleep(10);
					}
				}
			});
			return clientsConnected;
		}

		public void StartBenchmark()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].StartSendingMessages();
			}
		}

		public void StopBenchmark()
		{
		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].Disconnect();
			}

			return Task.CompletedTask;
		}

		public Task StopServer()
		{
			return echoServer.StopServerThread();
		}

		public Task StopClients()
		{
			return Task.CompletedTask;
		}

		public Task DisposeClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].Dispose();
			}

			var allDisposed = Task.Run(() =>
			{
				for (int i = 0; i < echoClients.Count; i++)
				{
					while (!echoClients[i].IsDisposed)
					{
						Thread.Sleep(10);
					}
				}
			});
			return allDisposed;
		}

		public Task DisposeServer()
		{
			echoServer.Dispose();

			return Task.CompletedTask;
		}

		public void Deinitialize()
		{
		}
	}
}