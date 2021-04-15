// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceBenchmarkConfig.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;

namespace NetworkBenchmark
{
	public class PerformanceBenchmarkConfig : ManualConfig
	{
		public PerformanceBenchmarkConfig()
		{
			Add(DefaultConfig.Instance);

			Job baseJob = Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(10)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true)
				.WithPlatform(Platform.X64);

			AddJob(baseJob.WithRuntime(CoreRuntime.Core50));

			ConfigHelper.AddDefaultColumns(this);
		}
	}
}
