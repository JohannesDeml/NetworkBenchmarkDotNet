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

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark: INetworkBenchmark
	{
		private BenchmarkConfiguration config;
		private BenchmarkData benchmarkData;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;

		public void Initialize(BenchmarkConfiguration config, BenchmarkData benchmarkData)
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

			var disposeAll = Task.Run(() =>
			{
				for (int i = 0; i < config.Clients; i++)
				{
					while (!echoClients[i].IsDisposed)
					{
						Thread.Sleep(10);
					}
				}
			});

			return disposeAll;
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
			
		}
	}
}
