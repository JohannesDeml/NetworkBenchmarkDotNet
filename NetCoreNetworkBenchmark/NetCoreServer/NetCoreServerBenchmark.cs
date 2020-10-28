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
using System.Threading.Tasks;

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark: INetworkBenchmark
	{
		private BenchmarkConfiguration config;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;

		public void Initialize(BenchmarkConfiguration config)
		{
			this.config = config;
			this.echoServer = new EchoServer(this.config);
			this.echoClients = new List<EchoClient>(config.NumClients);
		}

		public Task StartServer()
		{
			echoServer.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!echoServer.IsStarted)
				{
					Task.Delay(10);
				}
			});
			return serverStarted;
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.NumClients; i++)
			{
				echoClients.Add(new EchoClient(config));
			}

			return Task.CompletedTask;
		}

		public Task ConnectClients()
		{
			for (int i = 0; i < config.NumClients; i++)
			{
				echoClients[i].Connect();
			}

			var clientsConnected = Task.Run(() =>
			{
				for (int i = 0; i < config.NumClients; i++)
				{
					while (!echoClients[i].IsConnected)
					{
						Task.Delay(10);
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
					Task.Delay(10);
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
				for (int i = 0; i < config.NumClients; i++)
				{
					while (!echoClients[i].IsDisposed)
					{
						Task.Delay(10);
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
					Task.Delay(10);
				}
			});

			return disposeAll;
		}
	}
}
