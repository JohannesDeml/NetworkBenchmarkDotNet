// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreServerBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark : ANetworkBenchmark
	{
		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoServer(setup, statistics);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, setup, statistics);
		}
	}
}
