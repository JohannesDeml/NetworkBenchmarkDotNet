// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InDepthBenchmarkConfig.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public class InDepthBenchmarkConfig : ManualConfig
	{
		public InDepthBenchmarkConfig()
		{
			Add(DefaultConfig.Instance);

			Job baseJob = Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(20)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true);

			AddJob(baseJob
				.WithRuntime(CoreRuntime.Core50)
				.WithPlatform(Platform.X64));

			AddJob(baseJob
				.WithRuntime(CoreRuntime.Core31)
				.WithPlatform(Platform.X64));

			AddColumn(new NumClientsColumn());
			AddColumn(new MessagesPerSecondColumn());
			AddColumn(FixedColumn.VersionColumn);
			AddColumn(FixedColumn.OperatingSystemColumn);

			AddExporter(MarkdownExporter.GitHub);
			AddExporter(new CsvExporter(CsvSeparator.Comma, ConfigConstants.CsvStyle));

			AddDiagnoser(MemoryDiagnoser.Default);
			AddDiagnoser(new EventPipeProfiler(EventPipeProfile.GcVerbose));
			AddDiagnoser(new EventPipeProfiler(EventPipeProfile.CpuSampling));
		}
	}
}
