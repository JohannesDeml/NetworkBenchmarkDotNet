// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NetCoreNetworkBenchmark
{
	[Flags]
	public enum PredefinedBenchmark
	{
		/// <summary>
		/// Run benchmark defined by commandline args
		/// </summary>
		None = 0,
		/// <summary>
		/// Run Benchmark Performance1, Performance 2
		/// Benchmark wih high CCU and high message count to get accurate performance numbers
		/// </summary>
		Performance = 1 << 0,
		/// <summary>
		/// Run Benchmark Garbage
		/// Benchmark which collects GC information
		/// </summary>
		Garbage = 1 << 1,
		/// <summary>
		/// Run all predefined benchmarks
		/// </summary>
		All = 1 << 2 - 1
	}
}
