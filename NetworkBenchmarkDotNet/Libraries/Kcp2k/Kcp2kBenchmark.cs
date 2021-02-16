// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kcp2kBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark.Kcp2k
{
	internal class Kcp2kBenchmark : ANetworkBenchmark
	{
		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkData data)
		{
			return new EchoServer(setup, data);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkData data)
		{
			return new EchoClient(id, setup, data);
		}
	}
}
