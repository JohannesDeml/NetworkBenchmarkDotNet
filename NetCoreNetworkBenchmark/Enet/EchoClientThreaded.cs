using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoClientThreaded : EnetClient
	{

		private readonly Thread connectAndListenThread;

		public EchoClientThreaded(int id, BenchmarkConfiguration config) : base(id, config)
		{
			connectAndListenThread = new Thread(ConnectAndListen);
			connectAndListenThread.Name = $"ENet Client {id}";
			connectAndListenThread.IsBackground = true;
		}

		public override void Start()
		{
			connectAndListenThread.Start();
		}

		public override async void Dispose()
		{
			while (connectAndListenThread.IsAlive)
			{
				await Task.Delay(10);
			}

			base.Dispose();
		}
	}
}
