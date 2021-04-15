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

			AddColumn(new NumClientsColumn());
			AddColumn(new MessagesPerSecondColumn());
			AddColumn(FixedColumn.VersionColumn);
			AddColumn(FixedColumn.OperatingSystemColumn);
			AddColumn(FixedColumn.DateTimeColumn);

			AddExporter(MarkdownExporter.GitHub);
			AddExporter(new CsvExporter(CsvSeparator.Comma, ConfigConstants.CsvStyle));
		}
	}
}
