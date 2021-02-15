// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumClientsColumn.cs">
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

namespace NetworkBenchmark
{
	/// <summary>
	/// Show the number of clients used. In an own column, since params can't be sorted
	/// waiting for https://github.com/dotnet/BenchmarkDotNet/pull/1612
	/// </summary>
	public class NumClientsColumn : IColumn
	{
		public string Id => "Number of Clients";

		public string ColumnName => "Clients";

		public bool AlwaysShow => true;

		public ColumnCategory Category => ColumnCategory.Statistics;

		public int PriorityInCategory => int.MinValue;

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
			var report = summary[benchmarkCase];

			if (!type.IsSubclassOf(typeof(APredefinedBenchmark)))
			{
				return string.Empty;
			}

			if (!report.Success || !report.BuildResult.IsBuildSuccess || report.ExecuteResults.Count == 0)
			{
				return "NA";
			}

			var instance = (APredefinedBenchmark) Activator.CreateInstance(type);
			if (instance == null)
			{
				return "NA";
			}


			int numClients = instance.ClientCount;
			var cultureInfo = summary.GetCultureInfo();
			return numClients.ToString("N0", cultureInfo);
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
