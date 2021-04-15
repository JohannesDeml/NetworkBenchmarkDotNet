// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InDepthBenchmarkConfig.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
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
				.WithGcForce(true)
				.WithPlatform(Platform.X64);

			// Here you can test different runtimes
			AddJob(baseJob.WithRuntime(CoreRuntime.Core50));

			AddColumn(new MessagesPerSecondColumn());
			AddColumn(FixedColumn.VersionColumn);
			AddColumn(FixedColumn.OperatingSystemColumn);
			AddColumn(FixedColumn.DateTimeColumn);

			AddExporter(MarkdownExporter.GitHub);
			AddExporter(new CsvExporter(CsvSeparator.Comma, ConfigConstants.CsvStyle));

			// You can also use additional diagnosers.
			// Those might result in large trace files and can take some time to process after the benchmark finished
			// AddDiagnoser(MemoryDiagnoser.Default);
			// AddDiagnoser(new EventPipeProfiler(EventPipeProfile.GcVerbose));
			// AddDiagnoser(new EventPipeProfiler(EventPipeProfile.CpuSampling));
		}
	}
}
