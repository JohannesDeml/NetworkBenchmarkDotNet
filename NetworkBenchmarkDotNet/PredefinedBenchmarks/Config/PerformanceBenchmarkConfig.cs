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

			AddJob(Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(20)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true)
				.WithRuntime(CoreRuntime.Core50)
				.WithPlatform(Platform.X64));

			AddColumn(new NumClientsColumn());
			AddColumn(new MessagesPerSecondColumn());
			AddColumn(FixedColumn.VersionColumn);
			AddColumn(FixedColumn.OperatingSystemColumn);

			AddExporter(MarkdownExporter.GitHub);
			AddExporter(new CsvExporter(CsvSeparator.Comma, ConfigConstants.CsvStyle));
		}
	}
}
