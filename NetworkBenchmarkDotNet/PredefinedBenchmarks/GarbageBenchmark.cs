// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GarbageBenchmark.cs">
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
	[Config(typeof(GarbageBenchmarkConfig))]
	public class GarbageBenchmark : APredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib, NetworkLibrary.NetCoreServer)]
		public NetworkLibrary Library { get; set; }

		public override int ClientCount { get; set; } = 10;
		public override int MessageTarget { get; set; } = 10_000;
		protected override BenchmarkMode Mode => BenchmarkMode.Garbage;
		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(Garbage))]
		public void PrepareGarbageBenchmark()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 10;
			config.MessageByteSize = 128;
			PrepareBenchmark();
		}

		[Benchmark]
		public long Garbage()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "GarbageBenchmark";
		}
	}
}
