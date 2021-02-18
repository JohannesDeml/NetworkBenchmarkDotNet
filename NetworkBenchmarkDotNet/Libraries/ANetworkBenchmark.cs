// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ANetworkBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkBenchmark
{
	public abstract class ANetworkBenchmark : INetworkBenchmark
	{
		private Configuration config;
		private BenchmarkStatistics benchmarkStatistics;
		private IServer echoServer;
		private List<IClient> echoClients;


		public virtual void Initialize(Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			echoServer = CreateNewServer(config, benchmarkStatistics);
			echoClients = new List<IClient>();
		}

		protected abstract IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics);

		public Task StartServer()
		{
			echoServer.StartServer();
			return Utilities.WaitForServerToStart(echoServer);
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients.Add(CreateNewClient(i, config, benchmarkStatistics));
			}

			return Task.CompletedTask;
		}

		protected abstract IClient CreateNewClient(int id, Configuration config, BenchmarkStatistics statistics);

		public Task ConnectClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients[i].StartClient();
			}

			return Utilities.WaitForClientsToConnect(echoClients);
		}

		public void StartBenchmark()
		{
			echoServer.StartBenchmark();

			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].StartBenchmark();
			}
		}

		public void StopBenchmark()
		{
			echoServer.StopBenchmark();

			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].StopBenchmark();
			}
		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].DisconnectClient();
			}

			return Utilities.WaitForClientsToDisconnect(echoClients);
		}

		public Task StopServer()
		{
			echoServer.StopServer();
			return Utilities.WaitForServerToStop(echoServer);
		}

		public Task StopClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].StopClient();
			}

			return Utilities.WaitForClientsToStop(echoClients);
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

		public virtual void Deinitialize()
		{
			// Deinitialize the used library if applicable
		}
	}
}
