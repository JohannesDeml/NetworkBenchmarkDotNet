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
		public override void Initialize(BenchmarkSetup config, BenchmarkData benchmarkData)
		{
			ENet.Library.Initialize();
			base.Initialize(config, benchmarkData);
		}

		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkData data)
		{
			return new EchoServer(setup, data);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkData data)
		{
			return new EchoClient(id, setup, data);
		}

		public override void Deinitialize()
		{
			base.Deinitialize();
			ENet.Library.Deinitialize();
		}
	}
}
