// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreServerBenchmark.cs">
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

namespace NetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark : INetworkBenchmark
	{
		private BenchmarkSetup config;
		private BenchmarkData benchmarkData;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;

		public void Initialize(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			this.echoServer = new EchoServer(config, benchmarkData);
			this.echoClients = new List<EchoClient>(config.Clients);
		}

		public Task StartServer()
		{
			echoServer.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!echoServer.IsStarted)
				{
					Thread.Sleep(10);
				}
			});
			return serverStarted;
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients.Add(new EchoClient(config, benchmarkData));
			}

			return Task.CompletedTask;
		}

		public Task ConnectClients()
		{
			for (int i = 0; i < config.Clients; i++)
			{
				echoClients[i].Connect();
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
			echoServer.Stop();
			var serverStopped = Task.Run(() =>
			{
				while (echoServer.IsStarted)
				{
					Thread.Sleep(10);
				}
			});
			return serverStopped;
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

			return Utilities.WaitForClientsToDispose(echoClients);
		}

		public Task DisposeServer()
		{
			echoServer.Dispose();

			var disposeAll = Task.Run(() =>
			{
				while (!echoServer.IsDisposed)
				{
					Thread.Sleep(10);
				}
			});

			return disposeAll;
		}

		public void Deinitialize()
		{
			// Library does not need to be deinitialized
		}
	}
}
