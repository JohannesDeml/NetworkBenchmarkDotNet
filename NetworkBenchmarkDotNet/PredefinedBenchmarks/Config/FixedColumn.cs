// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedColumn.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NetworkBenchmark
{
	/// <summary>
	/// A simple column having one defined value for all cells
	/// Helpful when merging different csvs to still being able to distinguish them in e.g. their version or os
	/// By default it is set to not always show, since its main use case is to show metadata
	/// </summary>
	public class FixedColumn : IColumn
	{
		/// <summary>
		/// Column showing the application version
		/// </summary>
		public static readonly FixedColumn VersionColumn =
			new FixedColumn("Version", Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version);

		/// <summary>
		/// Column showing the used operating system
		/// </summary>
		public static readonly FixedColumn OperatingSystemColumn =
			new FixedColumn("OS", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
		public string Id { get; }
		public string ColumnName { get; }

		private readonly string cellValue;

		public FixedColumn(string columnName, string cellValue)
		{
			this.cellValue = cellValue;
			ColumnName = columnName;
			Id = nameof(FixedColumn) + "." + ColumnName;
		}

		public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
		public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => cellValue;

		public bool IsAvailable(Summary summary) => true;

		public bool AlwaysShow { get; set; } = false;
		public ColumnCategory Category => ColumnCategory.Custom;
		public int PriorityInCategory { get; set; } =  0;
		public bool IsNumeric => false;
		public UnitType UnitType => UnitType.Dimensionless;
		public string Legend => $"Fixed column with value {cellValue}";
		public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);
		public override string ToString() => ColumnName;
	}
}
