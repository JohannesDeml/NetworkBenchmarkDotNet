using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreNetworkBenchmark.LiteNetLib
{
	internal class LiteNetLibBenchmark: INetworkBenchmark
	{
		private BenchmarkConfiguration config;
		private EchoServer echoServer;
		private List<EchoClient> echoClients;


		public void Initialize(BenchmarkConfiguration config)
		{
			this.config = config;
			echoServer = new EchoServer(config);
			echoClients = new List<EchoClient>();
		}

		public Task StartServer()
		{
			return echoServer.StartServerThread();
		}

		public Task StartClients()
		{
			for (int i = 0; i < config.NumClients; i++)
			{
				echoClients.Add(new EchoClient(i, config));
			}

			return Task.CompletedTask;
		}

		public Task ConnectClients()
		{
			for (int i = 0; i < config.NumClients; i++)
			{
				echoClients[i].Start();
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

			var clientsDisconnected = Task.Run(() =>
			{
				for (int i = 0; i < config.NumClients; i++)
				{
					while (echoClients[i].IsConnected)
					{
						Task.Delay(10);
					}
				}
			});
			return clientsDisconnected;
		}

		public Task StopServer()
		{
			return echoServer.StopServerThread();
		}

		public Task StopClients()
		{
			for (int i = 0; i < echoClients.Count; i++)
			{
				echoClients[i].Stop();
			}

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
						Task.Delay(10);
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
	}
}
