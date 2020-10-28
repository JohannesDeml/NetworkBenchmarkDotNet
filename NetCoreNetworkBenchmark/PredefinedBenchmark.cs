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
	[SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 10)]
	[RPlotExporter]
	public class PredefinedBenchmark
	{
		//[Params(NetworkLibrary.ENet, NetworkLibrary.LiteNetLib, NetworkLibrary.NetCoreServer)]
		public NetworkLibrary Library = NetworkLibrary.LiteNetLib;

		[Params(100000)]
		public int MessageTarget;

		private INetworkBenchmark libraryImpl;

		[GlobalSetup]
		public void PrepareBenchmark()
		{
			Benchmark.Config.Verbose = false;
			Benchmark.Config.Library = Library;
			libraryImpl = INetworkBenchmark.CreateNetworkBenchmark(Library);
			Benchmark.PrepareBenchmark(libraryImpl);
		}

		[Benchmark]
		public long RunBenchmark()
		{
			var benchmarkdata = Benchmark.BenchmarkData;
			Benchmark.StartBenchmark(libraryImpl);
			var receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);

			while (receivedMessages < MessageTarget)
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
