// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using BenchmarkDotNet.Attributes;

namespace NetCoreNetworkBenchmark
{
	[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 10)]
	[GcServer(true)]
	[GcConcurrent(false)]
	[RPlotExporter]
	public class PredefinedBenchmark
	{
		[ParamsAllValues]
		public NetworkLibrary Library;

		private int messageTarget;
		private INetworkBenchmark libraryImpl;

		[GlobalSetup(Target = nameof(Benchmark1))]
		public void PrepareBenchmark1()
		{
			messageTarget = 1000 * 1000;
			var config = Benchmark.Config;
			config.Name = "1";
			config.NumClients = 1000;
			config.ParallelMessagesPerClient = 1;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(Benchmark2))]
		public void PrepareBenchmark2()
		{
			messageTarget = 1000 * 1000;
			var config = Benchmark.Config;
			config.Name = "2";
			config.NumClients = 100;
			config.ParallelMessagesPerClient = 10;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup]
		public void PrepareBenchmark()
		{
			var config = Benchmark.Config;
			config.Verbose = false;
			config.Library = Library;
			config.TestType = TestType.PingPong;
			config.MessagePayload = MessagePayload.Random;

			libraryImpl = INetworkBenchmark.CreateNetworkBenchmark(Library);
			Benchmark.PrepareBenchmark(libraryImpl);
		}

		[Benchmark]
		public long Benchmark1()
		{
			return RunBenchmark();
		}

		[Benchmark]
		public long Benchmark2()
		{
			return RunBenchmark();
		}

		private long RunBenchmark()
		{
			var benchmarkdata = Benchmark.BenchmarkData;
			Benchmark.StartBenchmark(libraryImpl);
			var receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);

			while (receivedMessages < messageTarget)
			{
				Thread.Sleep(1);
				receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);
			}

			Benchmark.StopBenchmark(libraryImpl);
			return receivedMessages;
		}

		[IterationCleanup]
		public void CleanupIteration()
		{
			// Wait for messages from previous benchmark to be all sent
			Thread.Sleep(100);
		}

		[GlobalCleanup]
		public void CleanupBenchmark()
		{
			Benchmark.CleanupBenchmark(libraryImpl);
		}
	}
}
