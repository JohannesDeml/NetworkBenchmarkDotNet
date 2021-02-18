// --------------------------------------------------------------------------------------------------------------------
// <copyright file="APredefinedBenchmark.cs">
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

namespace NetworkBenchmark
{
	public abstract class APredefinedBenchmark
	{
		public abstract int ClientCount { get; set; }
		public abstract int MessageTarget { get; set; }
		protected abstract BenchmarkMode Mode { get; }
		protected abstract NetworkLibrary LibraryTarget { get; }

		private INetworkBenchmark libraryImpl;


		[GlobalSetup]
		public void PrepareBenchmark()
		{
			var config = BenchmarkCoordinator.Config;
			config.Benchmark = Mode;
			config.Clients = ClientCount;
			config.Library = LibraryTarget;
			Console.Write(config.ToFormattedString());

			libraryImpl = INetworkBenchmark.CreateNetworkBenchmark(LibraryTarget);
			BenchmarkCoordinator.PrepareBenchmark(libraryImpl);
		}

		protected long RunBenchmark()
		{
			var statistics = BenchmarkCoordinator.BenchmarkStatistics;
			BenchmarkCoordinator.StartBenchmark(libraryImpl);
			var receivedMessages = Interlocked.Read(ref statistics.MessagesClientReceived);

			while (receivedMessages < MessageTarget)
			{
				Thread.Sleep(1);
				receivedMessages = Interlocked.Read(ref statistics.MessagesClientReceived);
			}

			BenchmarkCoordinator.StopBenchmark(libraryImpl);
			return receivedMessages;
		}

		[IterationCleanup]
		public void CleanupIteration()
		{
			// Wait for messages from previous benchmark to be all sent
			// TODO this can be done in a cleaner way
			Thread.Sleep(100);
		}

		[GlobalCleanup]
		public void CleanupBenchmark()
		{
			BenchmarkCoordinator.CleanupBenchmark(libraryImpl);
		}
	}
}
