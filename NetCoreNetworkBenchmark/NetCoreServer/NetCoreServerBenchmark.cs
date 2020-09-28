using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreNetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark: INetworkBenchmark
	{
		private BenchmarkConfiguration _config;
		private EchoServer _echoServer;
		private List<EchoClient> _echoClients;

		public void Initialize(BenchmarkConfiguration config)
		{
			this._config = config;
			this._echoServer = new EchoServer(_config);
			this._echoClients = new List<EchoClient>(config.NumClients);

		}

		public Task StartServer()
		{
			_echoServer.Start();
			var serverStarted = Task.Run(() =>
			{
				while (!_echoServer.IsStarted)
				{
					Task.Delay(10);
				}
			});
			return serverStarted;
		}

		public Task StartClients()
		{
			for (int i = 0; i < _config.NumClients; i++)
			{
				_echoClients.Add(new EchoClient(_config));
			}

			return Task.CompletedTask;
		}

		public Task ConnectClients()
		{
			for (int i = 0; i < _config.NumClients; i++)
			{
				_echoClients[i].Connect();
			}

			var clientsConnected = Task.Run(() =>
			{
				for (int i = 0; i < _config.NumClients; i++)
				{
					while (!_echoClients[i].IsConnected)
					{
						Task.Delay(10);
					}
				}

			});
			return clientsConnected;
		}

		public void StartBenchmark()
		{
			for (int i = 0; i < _echoClients.Count; i++)
			{
				_echoClients[i].StartSendingMessages();
			}
		}

		public void StopBenchmark()
		{

		}

		public Task DisconnectClients()
		{
			for (int i = 0; i < _echoClients.Count; i++)
			{
				_echoClients[i].Disconnect();
			}
			return Task.CompletedTask;
		}

		public Task StopServer()
		{
			_echoServer.Dispose();
			return Task.CompletedTask;
		}

		public Task StopClients()
		{
			return Task.CompletedTask;
		}

		public Task Dispose()
		{
			for (int i = 0; i < _echoClients.Count; i++)
			{
				_echoClients[i].Dispose();
			}
			_echoServer.Dispose();

			var disposed = Task.Run(() =>
			{
				while (!_echoServer.IsDisposed)
				{
					Task.Delay(10);
				}
				for (int i = 0; i < _config.NumClients; i++)
				{
					while (!_echoClients[i].IsDisposed)
					{
						Task.Delay(10);
					}
				}

			});

			return disposed;
		}
	}
}
