// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceBenchmarkConfig.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace NetworkBenchmark
{
	public class PerformanceBenchmarkConfig : ManualConfig
	{
		public PerformanceBenchmarkConfig()
		{
			Job baseJob = Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(10)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true);

			AddJob(baseJob.WithRuntime(CoreRuntime.Core60));

			ConfigHelper.AddDefaultColumns(this);
			AddColumn(new MessagesPerSecondColumn());
		}
	}
}
