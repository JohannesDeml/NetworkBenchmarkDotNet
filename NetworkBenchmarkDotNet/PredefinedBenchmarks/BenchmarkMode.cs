// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkMode.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NetworkBenchmark
{
	[Flags]
	public enum BenchmarkMode
	{
		/// <summary>
		/// Run benchmark defined by commandline args
		/// </summary>
		Custom = 0,

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
		/// Run all essential benchmarks (Performance, Garbage)
		/// </summary>
		Essential = (1 << 2) - 1,

		/// <summary>
		/// Run Benchmark In Depth
		/// Benchmark For ENEt with different settings and environments
		/// </summary>
		InDepth = 1 << 2,

		/// <summary>
		/// Run all predefined benchmarks
		/// </summary>
		All = (1 << 3) - 1,
	}
}
