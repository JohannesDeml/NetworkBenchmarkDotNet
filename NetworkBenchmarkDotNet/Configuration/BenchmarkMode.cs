// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkMode.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
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
		/// Runtime: ~15 minutes
		/// </summary>
		Performance = 1 << 0,

		/// <summary>
		/// Run Benchmark Garbage
		/// Benchmark which collects GC information
		/// Runtime: ~1 minute
		/// </summary>
		Garbage = 1 << 1,

		/// <summary>
		/// Run all essential benchmarks (Performance, Garbage)
		/// </summary>
		Essential = (1 << 2) - 1,

		/// <summary>
		/// Run a Quick Benchmark
		/// This benchmark might change over time and is kind of sandbox playground to test different settings with a fast run strategy.
		/// The results of this benchmark are not pre-jitted and are not as precise as the Essential benchmarks
		/// Runtime: ~30 seconds
		/// </summary>
		Quick = 1 << 2,

		/// <summary>
		/// Run all predefined benchmarks
		/// </summary>
		All = (1 << 3) - 1,
	}
}
