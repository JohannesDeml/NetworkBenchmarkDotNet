// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplingBenchmark.cs">
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
	[Config(typeof(SamplingBenchmarkConfig))]
	public class SamplingBenchmark : APredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib, NetworkLibrary.NetCoreServer)]
		public NetworkLibrary Library { get; set; }

		[Params(1, Priority = 100)]
		public override int ClientCount { get; set; }
		public override int MessageTarget { get; set; } = 100_000;
		protected override BenchmarkMode Mode => BenchmarkMode.Sampling;
		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(SampleSimpleEcho))]
		public void PrepareSamplingBenchmark()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 10;
			config.MessageByteSize = 128;
			PrepareBenchmark();
		}

		[Benchmark]
		public long SampleSimpleEcho()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "GarbageBenchmark";
		}
	}
}
