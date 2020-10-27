using System;
using System.Threading;
using System.Threading.Tasks;
using ENet;

namespace NetCoreNetworkBenchmark.Enet
{
	internal class EchoClient : EnetClient
	{
		private Task listenTask;

		public EchoClient(int id, BenchmarkConfiguration config) : base(id, config)
		{
		}

		public override void Start()
		{
			listenTask = Task.Factory.StartNew(ConnectAndListen, TaskCreationOptions.LongRunning);
		}

		public override async void Dispose()
		{
			while (!listenTask.IsCompleted)
			{
				await Task.Delay(10);
			}

			base.Dispose();
		}

	}
}
