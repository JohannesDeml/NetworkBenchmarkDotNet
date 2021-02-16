// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ENetBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkBenchmark.Enet
{
	internal class ENetBenchmark : INetworkBenchmark
	{
		private BenchmarkSetup config;
		private BenchmarkData benchmarkData;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;


		public void Initialize(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			ENet.Library.Initialize();
			echoServer = new EchoServer(config, benchmarkData);
			echoClients = new List<EchoClient>();
		}

		public Task StartServer()
		{
			echoServer.StartServerThread();
			return Utilities.WaitForServerToStart(echoServer);
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

			return Utilities.WaitForClientsToConnect(echoClients);
		}

		public void StartBenchmark()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].StartSendingMessages();
			}
		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].Disconnect();
			}

			return Utilities.WaitForClientsToDisconnect(echoClients);
		}

		public Task StopServer()
		{
			return Utilities.WaitForServerToStop(echoServer);
		}

		public Task StopClients()
		{
			return Utilities.WaitForClientThreadsToFinish(echoClients);
		}

		public Task DisposeClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].Dispose();
			}

			return Utilities.WaitForClientsToDispose(echoClients);
		}

		public Task DisposeServer()
		{
			echoServer.Dispose();

			return Task.CompletedTask;
		}

		public void Deinitialize()
		{
			ENet.Library.Deinitialize();
		}
	}
}
