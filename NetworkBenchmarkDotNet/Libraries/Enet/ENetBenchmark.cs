// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ENetBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------


namespace NetworkBenchmark.Enet
{
	internal class ENetBenchmark : ANetworkBenchmark
	{
		public override void Initialize(BenchmarkSetup config, BenchmarkStatistics benchmarkStatistics)
		{
			ENet.Library.Initialize();
			base.Initialize(config, benchmarkStatistics);
		}

		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoServer(setup, statistics);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, setup, statistics);
		}

		public override void Deinitialize()
		{
			base.Deinitialize();
			ENet.Library.Deinitialize();
		}
	}
}
