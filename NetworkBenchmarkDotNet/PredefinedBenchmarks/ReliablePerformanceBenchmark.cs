// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReliablePerformanceBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Attributes;

namespace NetworkBenchmark
{
	[Config(typeof(PerformanceBenchmarkConfig))]
	public class ReliablePerformanceBenchmark : APredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib)]
		public NetworkLibrary Library { get; set; }

		[Params(TransmissionType.Reliable)]
		public TransmissionType Transmission { get; set; }

		protected override BenchmarkMode Mode => BenchmarkMode.Performance;
		public override int ClientCount { get; set; } = 500;
		public override int MessageTarget { get; set; } = 500_000;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(PingPongReliable))]
		public void PreparePingPongReliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			config.Transmission = Transmission;
			PrepareBenchmark();
		}

		[Benchmark]
		public long PingPongReliable()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "ReliablePerformanceBenchmark";
		}
	}
}
