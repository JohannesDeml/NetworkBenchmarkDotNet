// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreliablePerformanceBenchmark.cs">
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
	[Config(typeof(PerformanceBenchmarkConfig))]
	public class UnreliablePerformanceBenchmark : APredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib, NetworkLibrary.NetCoreServer)]
		public NetworkLibrary Library { get; set; }

		[Params(TransmissionType.Unreliable)]
		public TransmissionType Transmission { get; set; }

		protected override BenchmarkMode Mode => BenchmarkMode.Performance;
		public override int ClientCount { get; set; } = 500;
		public override int MessageTarget { get; set; } = 500_000;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(PingPongUnreliable))]
		public void PreparePingPongUnreliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			config.Transmission = Transmission;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(PingPongBatchedUnreliable))]
		public void PreparePingPongBatchedUnreliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 10;
			config.MessageByteSize = 32;
			config.Transmission = Transmission;
			PrepareBenchmark();
		}


		[Benchmark]
		public long PingPongUnreliable()
		{
			return RunBenchmark();
		}

		[Benchmark]
		public long PingPongBatchedUnreliable()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "UnreliablePerformanceBenchmark";
		}
	}
}
