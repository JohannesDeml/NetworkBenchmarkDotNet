// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkConfig.cs">
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
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public class BenchmarkConfig : ManualConfig
	{
		public BenchmarkConfig()
		{
			Job baseConfig = Job.Default
				.WithLaunchCount(1)
				.WithWarmupCount(1)
				.WithIterationCount(10)
				.WithGcServer(true)
				.WithGcConcurrent(true)
				.WithGcForce(true);

			AddJob(baseConfig
				.WithRuntime(CoreRuntime.Core50)
				.WithPlatform(Platform.X64));

			AddExporter(MarkdownExporter.GitHub);
			var processableStyle = new SummaryStyle(CultureInfo.InvariantCulture, false, SizeUnit.KB, TimeUnit.Microsecond,
				false, true, 100);
			AddExporter(new CsvExporter(CsvSeparator.Comma, processableStyle));
		}
	}
}
