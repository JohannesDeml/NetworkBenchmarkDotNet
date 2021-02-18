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
	public class BenchmarkStatistics
	{
		public TimeSpan Duration { get; private set; }

		private readonly Stopwatch stopwatch;

		public long MessagesClientSent;
		public long MessagesClientReceived;
		public long MessagesServerSent;
		public long MessagesServerReceived;
		public long Errors;

		public BenchmarkStatistics()
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

		public void StartBenchmark()
		{
			stopwatch.Start();
		}

		public void StopBenchmark()
		{
			stopwatch.Stop();
			Duration = stopwatch.Elapsed;
		}
	}
}
