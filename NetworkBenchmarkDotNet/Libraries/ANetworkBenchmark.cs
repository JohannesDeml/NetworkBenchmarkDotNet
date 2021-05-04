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
		public IServer Server => server;
		public List<IClient> Clients => clients;

		private Configuration config;
		private BenchmarkStatistics benchmarkStatistics;
		private IServer server;
		private List<IClient> clients;



		public virtual void Initialize(Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			server = CreateNewServer(config, benchmarkStatistics);
			clients = new List<IClient>();
		}

		protected abstract IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics);

		public Task StartServer()
		{
			server.StartServer();
			return Utilities.WaitForServerToStart(server);
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				clients.Add(CreateNewClient(i, config, benchmarkStatistics));
			}

			return Task.CompletedTask;
		}

		protected abstract IClient CreateNewClient(int id, Configuration config, BenchmarkStatistics statistics);

		public Task ConnectClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				clients[i].StartClient();
			}

			return Utilities.WaitForClientsToConnect(clients);
		}

		public void StartBenchmark()
		{
			server.StartBenchmark();

			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].StartBenchmark();
			}
		}

		public void StopBenchmark()
		{
			server.StopBenchmark();

			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].StopBenchmark();
			}
		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].DisconnectClient();
			}

			return Utilities.WaitForClientsToDisconnect(clients);
		}

		public Task StopServer()
		{
			server.StopServer();
			return Utilities.WaitForServerToStop(server);
		}

		public Task StopClients()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].StopClient();
			}

			return Utilities.WaitForClientsToStop(clients);
		}

		public Task DisposeClients()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].Dispose();
			}

			return Utilities.WaitForClientsToDispose(clients);
		}

		public Task DisposeServer()
		{
			server.Dispose();

			return Task.CompletedTask;
		}

		public virtual void Deinitialize()
		{
			// Deinitialize the used library if applicable
		}
	}
}
