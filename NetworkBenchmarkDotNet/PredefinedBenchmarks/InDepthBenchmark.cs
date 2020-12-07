// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InDepthBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Attributes;

namespace NetworkBenchmark
{
	[Config(typeof(InDepthBenchmarkConfig))]
	public class InDepthBenchmark : APredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet)]
		public NetworkLibrary Library { get; set; }

		[Params("127.0.0.1", "::1")]
		public string Address { get; set; }

		[Params(30, 50, 60)]
		public int TickRate { get; set; }

		protected override BenchmarkMode Mode => BenchmarkMode.InDepth;
		public override int ClientCount => 500;
		public override int MessageTarget => 1000 * 500;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(InDepth))]
		public void PrepareInDepthBenchmark()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ClientTickRate = TickRate;
			config.ServerTickRate = TickRate;
			config.Address = Address;
			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[Benchmark]
		public long InDepth()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "InDepthBenchmark";
		}
	}
}
