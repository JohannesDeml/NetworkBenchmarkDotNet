// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigConstants.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public static class ConfigConstants
	{
		/// <summary>
		/// A summary style that makes processing of data more accessible.
		/// * Long column width to avoid names being truncated
		/// * units stay the same
		/// * No units in cell data (Always numbers)
		/// </summary>
		public static readonly SummaryStyle CsvStyle = new SummaryStyle(CultureInfo.InvariantCulture, false, SizeUnit.KB, TimeUnit.Millisecond,
			false, true, 100);
	}
}
