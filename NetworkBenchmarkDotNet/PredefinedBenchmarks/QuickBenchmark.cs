// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickBenchmark.cs">
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
	[Config(typeof(QuickBenchmarkConfig))]
	public class QuickBenchmark : APredefinedBenchmark
	{
		/// <summary>
		/// Library target for the benchmark
		/// </summary>
		[Params(NetworkLibrary.Telepathy)]
		public NetworkLibrary Library { get; set; }

		/// <summary>
		/// Address to use, supports ipv4 and ipv6
		/// If the address is not localhost, the execution-mode is switched to Client
		/// </summary>
		[Params("::1")]
		public string Address { get; set; }

		[Params(120)]
		public int TickRate { get; set; }

		[Params(100, 1000)]
		public override int ClientCount { get; set; }

		[Params(TransmissionType.Reliable)]
		public TransmissionType Transmission { get; set; }

		[Params(100_000)]
		public override int MessageTarget { get; set; }

		protected override BenchmarkMode Mode => BenchmarkMode.Quick;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(Quick))]
		public void PrepareInDepthBenchmark()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ClientTickRate = TickRate;
			config.ServerTickRate = TickRate;
			config.Address = Address;
			if (Address != "::1" && Address != "127.0.0.1")
			{
				config.ExecutionMode = ExecutionMode.Client;
			}

			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			config.Transmission = Transmission;
			PrepareBenchmark();
		}

		[Benchmark]
		public long Quick()
		{
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "QuickBenchmark";
		}
	}
}
