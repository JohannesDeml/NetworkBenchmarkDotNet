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
	[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 10, id:"Performance Benchmark")]
	[RPlotExporter]
	public class PerformanceBenchmark : APredefinedBenchmark
	{
		[GlobalSetup(Target = nameof(Performance1))]
		public void PreparePerformanceBenchmark1()
		{
			Benchmark.ApplyPredefinedConfiguration();
			var config = Benchmark.Config;

			MessageTarget = 1000 * 1000;
			config.Clients = 1000;
			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(Performance2))]
		public void PreparePerformanceBenchmark2()
		{
			Benchmark.ApplyPredefinedConfiguration();
			var config = Benchmark.Config;

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
