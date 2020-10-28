// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Benchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;

namespace NetCoreNetworkBenchmark
{
	public static class Benchmark
	{
		public static readonly BenchmarkConfiguration Config = new BenchmarkConfiguration();

		public static void PrepareBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Prepare Benchmark.");
			Config.PrepareForNewBenchmark();
			networkBenchmark.Initialize(Config);
			Utilities.WriteVerbose(".");

			var serverTask = networkBenchmark.StartServer();
			var clientTask = networkBenchmark.StartClients();
			serverTask.Wait();
			clientTask.Wait();
			Utilities.WriteVerbose(".");

			networkBenchmark.ConnectClients().Wait();
			Utilities.WriteVerboseLine(" Done");
		}

		public static void RunTimedBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose($"-> Run Benchmark {Config.Library}...");
			Benchmark.StartBenchmark(networkBenchmark);

			Thread.Sleep(Config.TestDurationInSeconds * 1000);

			Benchmark.StopBenchmark(networkBenchmark);
			Utilities.WriteVerboseLine(" Done");
		}

		public static void StartBenchmark(INetworkBenchmark networkBenchmark)
		{
			Config.BenchmarkData.Reset();
			Config.BenchmarkData.StartBenchmark();
			networkBenchmark.StartBenchmark();
		}

		public static void StopBenchmark(INetworkBenchmark networkBenchmark)
		{
			networkBenchmark.StopBenchmark();
			Config.BenchmarkData.StopBenchmark();
		}

		public static void CleanupBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Clean up.");
			networkBenchmark.DisconnectClients().Wait();
			networkBenchmark.StopClients().Wait();
			networkBenchmark.DisposeClients().Wait();
			Utilities.WriteVerbose(".");


			networkBenchmark.StopServer().Wait();
			Utilities.WriteVerbose(".");
			networkBenchmark.DisposeServer().Wait();
			Utilities.WriteVerboseLine(" Done");
		}
	}
}
