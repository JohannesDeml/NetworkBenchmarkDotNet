// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CcuBenchmark.cs">
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
	[Config(typeof(CcuBenchmarkConfig))]
	public class CcuBenchmark : APredefinedBenchmark
	{
		/// <summary>
		/// Library target for the benchmark
		/// </summary>
		[Params(NetworkLibrary.LiteNetLib)]
		public NetworkLibrary Library { get; set; }

		[Params(32, 500)]
		public int MessageByteSize { get; set; }

		[Params(100, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000)]
		public override int ClientCount { get; set; }

		[Params(TransmissionType.Unreliable)]
		public TransmissionType Transmission { get; set; }

		[Params(100_000)]
		public override int MessageTarget { get; set; }

		protected override BenchmarkMode Mode => BenchmarkMode.Ccu;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(Ccu))]
		public void PrepareCcuBenchmark()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 1;
			config.MessageByteSize = MessageByteSize;
			config.Transmission = Transmission;
			PrepareBenchmark();
		}

		[Benchmark]
		public long Ccu()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "CcuBenchmark";
		}
	}
}
