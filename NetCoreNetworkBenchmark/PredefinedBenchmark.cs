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
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace NetCoreNetworkBenchmark
{
	[SimpleJob(launchCount: 0, warmupCount: 0, targetCount: 10)]
	public class PredefinedBenchmark
	{
		[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib, NetworkLibrary.NetCoreServer)]
		public NetworkLibrary Library;

		[Params(100000)]
		public int MessageTarget;

		private INetworkBenchmark libraryImpl;

		[GlobalSetup]
		public void PrepareBenchmark()
		{
			Benchmark.Config.Library = Library;
			libraryImpl = INetworkBenchmark.CreateNetworkBenchmark(Library);
			Benchmark.PrepareBenchmark(libraryImpl);
		}

		[Benchmark]
		public long RunBenchmark()
		{
			var benchmarkdata = Benchmark.BenchmarkData;
			Benchmark.StartBenchmark(libraryImpl);
			while (benchmarkdata.MessagesClientReceived < MessageTarget)
			{
				Thread.Sleep(1);
			}
			Benchmark.StopBenchmark(libraryImpl);
			return benchmarkdata.MessagesClientReceived;
		}

		[IterationCleanup]
		public void CleanupIteration()
		{
			// Wait for messages from previous benchmark to be all sent
			Thread.Sleep(50);
		}

		[GlobalCleanup]
		public void CleanupBenchmark()
		{
			Benchmark.CleanupBenchmark(libraryImpl);
		}
	}
}
