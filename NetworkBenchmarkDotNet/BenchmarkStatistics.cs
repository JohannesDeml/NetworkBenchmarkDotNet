// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkStatistics.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public class BenchmarkStatistics
	{
		private readonly Stopwatch stopwatch;
		private TimeSpan duration;

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
			duration = stopwatch.Elapsed;
		}

		public string PrintStatistics(Configuration config)
		{
			var sb = new StringBuilder();

			sb.AppendLine("```");
			sb.AppendLine($"Results {config.Library} with {config.Transmission} {config.Test}");
			if (Errors > 0)
			{
				sb.AppendLine($"Errors: {Errors}");
				sb.AppendLine();
			}

			sb.AppendLine($"Duration: {duration.TotalSeconds:0.000} s");
			sb.AppendLine($"Messages sent by clients: {MessagesClientSent:n0}");
			sb.AppendLine($"Messages server received: {MessagesServerReceived:n0}");
			sb.AppendLine($"Messages sent by server: {MessagesServerSent:n0}");
			sb.AppendLine($"Messages clients received: {MessagesClientReceived:n0}");
			sb.AppendLine();

			var totalBytes = MessagesClientReceived * config.MessageByteSize;
			var totalMb = totalBytes / (1024.0d * 1024.0d);
			var clientRtt = new TimeInterval(duration.TotalMilliseconds * config.Clients / MessagesClientReceived,
				TimeUnit.Millisecond);

			sb.AppendLine($"Total data: {totalMb:0.00} MB");
			sb.AppendLine($"Data throughput: {totalMb / duration.TotalSeconds:0.00} MB/s");
			sb.AppendLine($"Message throughput: {MessagesClientReceived / duration.TotalSeconds:n0} msg/s");
			if (config.ParallelMessages == 1)
			{
				sb.AppendLine($"Client Round Trip Time: {clientRtt.ToString()}");
			}

			sb.AppendLine("```");
			sb.AppendLine();

			return sb.ToString();
		}
	}
}
