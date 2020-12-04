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
		public abstract int MessageTarget { get; }
		protected abstract NetworkLibrary LibraryTarget { get; }

		private INetworkBenchmark libraryImpl;


		[GlobalSetup]
		public void PrepareBenchmark()
		{
			var config = BenchmarkCoordinator.Config;
			config.Library = LibraryTarget;
			Console.Write(config.PrintSetup());

			libraryImpl = INetworkBenchmark.CreateNetworkBenchmark(LibraryTarget);
			BenchmarkCoordinator.PrepareBenchmark(libraryImpl);
		}

		protected long RunBenchmark()
		{
			var benchmarkdata = BenchmarkCoordinator.BenchmarkData;
			BenchmarkCoordinator.StartBenchmark(libraryImpl);
			var receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);

			while (receivedMessages < MessageTarget)
			{
				Thread.Sleep(1);
				receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);
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
