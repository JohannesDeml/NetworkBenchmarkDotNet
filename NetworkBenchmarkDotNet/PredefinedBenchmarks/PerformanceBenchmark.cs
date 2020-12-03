// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedBenchmark.cs">
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
	[Config(typeof(BenchmarkConfig))]
	[RPlotExporter]
	public class PerformanceBenchmark : APredefinedBenchmark
	{
		[GlobalSetup(Target = nameof(Performance1))]
		public void PreparePerformanceBenchmark1()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			MessageTarget = 1000 * 1000;
			config.Clients = 1000;
			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(Performance2))]
		public void PreparePerformanceBenchmark2()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			MessageTarget = 1000 * 1000;
			config.Clients = 1000;
			config.ParallelMessages = 10;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[Benchmark]
		public long Performance1()
		{
			return RunBenchmark();
		}

		[Benchmark]
		public long Performance2()
		{
			return RunBenchmark();
		}
	}
}
