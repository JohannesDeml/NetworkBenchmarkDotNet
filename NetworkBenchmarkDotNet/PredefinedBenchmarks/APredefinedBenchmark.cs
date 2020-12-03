// --------------------------------------------------------------------------------------------------------------------
// <copyright file="APredefinedBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using BenchmarkDotNet.Attributes;

namespace NetworkBenchmark
{
	[GcServer(true)]
	[GcConcurrent(false)]
	public abstract class APredefinedBenchmark
	{
		[ParamsAllValues]
		public NetworkLibrary Library;

		protected int MessageTarget;
		protected INetworkBenchmark LibraryImpl;


		[GlobalSetup]
		public void PrepareBenchmark()
		{
			var config = BenchmarkCoordinator.Config;
			config.Library = Library;

			LibraryImpl = INetworkBenchmark.CreateNetworkBenchmark(Library);
			BenchmarkCoordinator.PrepareBenchmark(LibraryImpl);
		}

		protected long RunBenchmark()
		{
			var benchmarkdata = BenchmarkCoordinator.BenchmarkData;
			BenchmarkCoordinator.StartBenchmark(LibraryImpl);
			var receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);

			while (receivedMessages < MessageTarget)
			{
				Thread.Sleep(1);
				receivedMessages = Interlocked.Read(ref benchmarkdata.MessagesClientReceived);
			}

			BenchmarkCoordinator.StopBenchmark(LibraryImpl);
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
			BenchmarkCoordinator.CleanupBenchmark(LibraryImpl);
		}
	}
}
