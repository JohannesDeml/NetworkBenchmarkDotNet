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

			if (type == null)
			{
				return "No type set";
			}

			if (type != typeof(PerformanceBenchmark))
			{
				return $"Not type of {nameof(PerformanceBenchmark)}";
			}

			var instance = (PerformanceBenchmark) Activator.CreateInstance(type);
			if (instance == null)
			{
				return "Could not create instance";
			}

			try
			{
				int numClients = instance.Clients;
				var cultureInfo = summary.GetCultureInfo();
				return numClients.ToString("N0", cultureInfo);
			}
			catch (Exception e)
			{
				return "Error";
			}
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
