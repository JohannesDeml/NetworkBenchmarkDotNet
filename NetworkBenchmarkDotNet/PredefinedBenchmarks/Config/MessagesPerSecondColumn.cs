// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagesPerSecondColumn.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public class MessagesPerSecondColumn : IColumn
	{
		public string Id => "Message Throughput";

		public string ColumnName => "Throughput";

		public bool AlwaysShow => true;

		public ColumnCategory Category => ColumnCategory.Statistics;

		public int PriorityInCategory => int.MinValue + 10;

		public bool IsNumeric => true;

		public UnitType UnitType => UnitType.Dimensionless;

		public string Legend => null;

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
		{
			return this.GetValue(summary, benchmarkCase, null);
		}

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
		{
			var type = benchmarkCase.Descriptor.Type;

			if (type == null)
			{
				return "No type set";
			}

			if (!type.IsSubclassOf(typeof(APredefinedBenchmark)))
			{
				return $"Not subclass of {nameof(APredefinedBenchmark)}";
			}

			var instance = (APredefinedBenchmark) Activator.CreateInstance(type);
			if (instance == null)
			{
				return "Could not create instance";
			}

			int messageCount = instance.MessageTarget;
			var statistics = summary[benchmarkCase].ResultStatistics;
			var meanSeconds = TimeUnit.Convert(statistics.Mean, TimeUnit.Nanosecond, TimeUnit.Second);
			var msgPerSecond = messageCount / meanSeconds;


			var cultureInfo = summary.GetCultureInfo();
			if (style.PrintUnitsInContent)
				return msgPerSecond.ToString("N0", cultureInfo) + " msg/s";

			return msgPerSecond.ToString("N0", cultureInfo);
		}

		public bool IsAvailable(Summary summary)
		{
			return true;
		}

		public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
		{
			return false;
		}
	}
}
