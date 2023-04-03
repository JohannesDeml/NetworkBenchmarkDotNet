// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplingBenchmarkConfig.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Tracing;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing.Parsers;

namespace NetworkBenchmark
{
	public class SamplingBenchmarkConfig : ManualConfig
	{
		public SamplingBenchmarkConfig()
		{
			Add(DefaultConfig.Instance);

			Job baseJob = Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(1)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true);

			AddJob(baseJob.WithRuntime(CoreRuntime.Core60));

			ConfigHelper.AddDefaultColumns(this);

			var providers = new[]
			{
				new EventPipeProvider(
					name: ClrTraceEventParser.ProviderName,
					eventLevel: EventLevel.Verbose,
					keywords: (long) ClrTraceEventParser.Keywords.Default |
					          (long) ClrTraceEventParser.Keywords.GC |
					          (long) ClrTraceEventParser.Keywords.GCHandle |
					          (long) ClrTraceEventParser.Keywords.Exception
				),
			};

			AddDiagnoser(new EventPipeProfiler(providers: providers, performExtraBenchmarksRun: false));
		}
	}
}
