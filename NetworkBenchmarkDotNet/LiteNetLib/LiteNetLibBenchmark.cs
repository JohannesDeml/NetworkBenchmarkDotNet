// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteNetLibBenchmark.cs">
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

namespace NetCoreNetworkBenchmark.LiteNetLib
{
	internal class LiteNetLibBenchmark: INetworkBenchmark
	{
		private BenchmarkConfiguration config;
		private BenchmarkData benchmarkData;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;


		public void Initialize(BenchmarkConfiguration config, BenchmarkData benchmarkData)
		{
			this.config = config;
			this.benchmarkData = benchmarkData;
			echoServer = new EchoServer(config, benchmarkData);
			echoClients = new List<EchoClient>();
		}

		public Task StartServer()
		{
			return echoServer.StartServer();
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
			// Triggers Updates on the main thread, therefore it takes some time for all clients to send messages
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
			var disconnectTasks = new List<Task>();
			for (int i = 0; i < echoClients.Count; i++)
			{
				disconnectTasks.Add(echoClients[i].Disconnect());
			}

			return Task.WhenAll(disconnectTasks);
		}

		public Task StopServer()
		{
			return echoServer.StopServer();
		}

		public Task StopClients()
		{
			var stopTasks = new List<Task>();
			for (int i = 0; i < echoClients.Count; i++)
			{
				stopTasks.Add(echoClients[i].Stop());
			}

			return Task.WhenAll(stopTasks);
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
