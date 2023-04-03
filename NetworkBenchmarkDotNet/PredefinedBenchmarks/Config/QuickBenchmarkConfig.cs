// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickBenchmarkConfig.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace NetworkBenchmark
{
	public class QuickBenchmarkConfig : ManualConfig
	{
		public QuickBenchmarkConfig()
		{
			Add(DefaultConfig.Instance);

			Job baseJob = Job.Default
				.WithStrategy(RunStrategy.Monitoring)
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(5)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true);

			// Here you can test different runtimes
			AddJob(baseJob.WithRuntime(CoreRuntime.Core60));

			ConfigHelper.AddDefaultColumns(this);
			AddColumn(new MessagesPerSecondColumn());

			// You can also use additional diagnosers.
			// Those might result in large trace files and can take some time to process after the benchmark finished
			// AddDiagnoser(MemoryDiagnoser.Default);
			// AddDiagnoser(new EventPipeProfiler(EventPipeProfile.GcVerbose));
			// AddDiagnoser(new EventPipeProfiler(EventPipeProfile.CpuSampling));
		}
	}
}
