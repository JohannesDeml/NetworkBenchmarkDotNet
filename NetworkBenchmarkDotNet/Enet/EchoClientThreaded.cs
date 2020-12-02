// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoClientThreaded.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace NetworkBenchmark.Enet
{
	internal class EchoClientThreaded: EnetClient
	{
		private readonly Thread connectAndListenThread;

		public EchoClientThreaded(int id, BenchmarkConfiguration config, BenchmarkData benchmarkData): base(id, config, benchmarkData)
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
