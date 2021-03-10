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
		private List<ClientGroup> clientGroups;


		public virtual void Initialize(Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;
			echoServer = CreateNewServer(config, benchmarkStatistics);
			clientGroups = new List<ClientGroup>();
		}

		protected abstract IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics);

		public Task StartServer()
		{
			echoServer.StartServer();
			return Utilities.WaitForServerToStart(echoServer);
		}

		// TODO no task
		public Task InitializeClients()
		{
			int clientId = 0;
			int clientsPerGroupLower = config.Clients / config.ClientGroups;
			int groupsWithAdditionalClient = config.Clients % config.ClientGroups;

			for (int i = 0; i < config.ClientGroups; i++)
			{
				var group = new ClientGroup(i, config, benchmarkStatistics);
				var clientCount = (i < groupsWithAdditionalClient)? clientsPerGroupLower + 1 :clientsPerGroupLower;
				group.InitializeClients(ref clientId, clientCount, this);
				clientGroups.Add(group);
			}

			return Task.CompletedTask;
		}

		public abstract IClient CreateNewClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics statistics);

		public Task ConnectClients()
		{
			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].StartClients();
			}

			return Utilities.WaitForClientGroupsToConnect(clientGroups);
		}

		public void StartBenchmark()
		{
			echoServer.StartBenchmark();

			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].StartBenchmark();
			}
		}

		public void StopBenchmark()
		{
			echoServer.StopBenchmark();

			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].StopBenchmark();
			}
		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].DisconnectClients();
			}

			return Utilities.WaitForClientGroupsToDisconnect(clientGroups);
		}

		public Task StopServer()
		{
			echoServer.StopServer();
			return Utilities.WaitForServerToStop(echoServer);
		}

		public Task StopClients()
		{
			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].StopClients();
			}

			return Utilities.WaitForClientGroupsToStop(clientGroups);
		}

		public Task DisposeClients()
		{
			for (int i = 0; i < clientGroups.Count; i++)
			{
				clientGroups[i].Dispose();
			}

			return Utilities.WaitForClientGroupsToDispose(clientGroups);
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
