// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkData.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace NetworkBenchmark
{
	public class BenchmarkData
	{
		public TimeSpan Duration { get; private set; }

		private readonly Stopwatch stopwatch;

		/// <summary>
		/// Benchmark is preparing to be run
		/// </summary>
		private bool preparing;

		/// <summary>
		/// Should server and clients listen for incoming messages
		/// </summary>
		private bool listen;

		/// <summary>
		/// Is a benchmark running (and therefore messages should be counted)
		/// </summary>
		private bool running;

		public long MessagesClientSent;
		public long MessagesClientReceived;
		public long MessagesServerSent;
		public long MessagesServerReceived;
		public long Errors;

		public BenchmarkData()
		{
			stopwatch = new Stopwatch();
		}

		public void Reset()
		{
			stopwatch.Reset();
			MessagesClientSent = 0L;
			MessagesClientReceived = 0L;
			MessagesServerSent = 0L;
			MessagesServerReceived = 0L;
			Errors = 0;
		}

		public void PrepareBenchmark()
		{
			listen = true;
			preparing = true;
		}

		public void StartBenchmark()
		{
			stopwatch.Start();
			running = true;
			preparing = false;
		}

		public void StopBenchmark()
		{
			running = false;
			stopwatch.Stop();
			Duration = stopwatch.Elapsed;
		}

		public void CleanupBenchmark()
		{
			listen = false;
		}
	}
}
