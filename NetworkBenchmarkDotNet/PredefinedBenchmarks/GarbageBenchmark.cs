// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GcBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace NetCoreNetworkBenchmark
{
	[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 10, id:"Garbage Collection Benchmark")]
	[EventPipeProfiler(EventPipeProfile.GcVerbose)]
	public class GarbageBenchmark : APredefinedBenchmark
	{
		[GlobalSetup(Target = nameof(Garbage))]
		public void PrepareGarbageBenchmark()
		{
			Benchmark.ApplyPredefinedConfiguration();
			var config = Benchmark.Config;

			MessageTarget = 1000 * 10;
			config.Clients = 10;
			config.ParallelMessages = 10;
			config.MessageByteSize = 128;
			PrepareBenchmark();
		}

		[Benchmark]
		public long Garbage()
		{
			return RunBenchmark();
		}
	}
}
