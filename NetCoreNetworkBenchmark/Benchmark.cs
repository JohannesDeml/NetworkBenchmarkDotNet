// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Benchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Threading;

namespace NetCoreNetworkBenchmark
{
	public static class Benchmark
	{
		public static BenchmarkConfiguration Config;
		public static readonly BenchmarkData BenchmarkData = new BenchmarkData();

		public static void ApplyPredefinedConfiguration()
		{
			if (Config == null)
			{
				Config = new BenchmarkConfiguration();
			}

			BenchmarkConfiguration.ApplyPredefinedBenchmarkConfiguration(ref Config);
		}

		public static void PrepareBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Prepare Benchmark.");
			Config.PrepareForNewBenchmark();
			BenchmarkData.PrepareBenchmark();
			networkBenchmark.Initialize(Config, BenchmarkData);
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

			Thread.Sleep(Config.Duration * 1000);

			Benchmark.StopBenchmark(networkBenchmark);
			Utilities.WriteVerboseLine(" Done");
		}

		public static void StartBenchmark(INetworkBenchmark networkBenchmark)
		{
			BenchmarkData.Reset();
			BenchmarkData.StartBenchmark();
			networkBenchmark.StartBenchmark();
		}

		public static void StopBenchmark(INetworkBenchmark networkBenchmark)
		{
			networkBenchmark.StopBenchmark();
			BenchmarkData.StopBenchmark();
		}

		public static void CleanupBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Clean up.");
			networkBenchmark.DisconnectClients().Wait();
			BenchmarkData.CleanupBenchmark();

			networkBenchmark.StopClients().Wait();
			networkBenchmark.DisposeClients().Wait();
			Utilities.WriteVerbose(".");


			networkBenchmark.StopServer().Wait();
			Utilities.WriteVerbose(".");
			networkBenchmark.DisposeServer().Wait();
			networkBenchmark.Deinitialize();
			Utilities.WriteVerboseLine(" Done");
			Utilities.WriteVerboseLine("");
		}

		public static string PrintStatistics()
		{
			var sb = new StringBuilder();

			sb.AppendLine("```");
			sb.AppendLine($"Results {Config.Test} with {Config.Library}");
			if (BenchmarkData.Errors > 0)
			{
				sb.AppendLine($"Errors: {BenchmarkData.Errors}");
				sb.AppendLine();
			}

			sb.AppendLine($"Duration: {BenchmarkData.Duration.TotalSeconds:0.000} s");
			sb.AppendLine($"Messages sent by clients: {BenchmarkData.MessagesClientSent:n0}");
			sb.AppendLine($"Messages server received: {BenchmarkData.MessagesServerReceived:n0}");
			sb.AppendLine($"Messages sent by server: {BenchmarkData.MessagesServerSent:n0}");
			sb.AppendLine($"Messages clients received: {BenchmarkData.MessagesClientReceived:n0}");
			sb.AppendLine();

			var totalBytes = BenchmarkData.MessagesClientReceived * Config.MessageByteSize;
			var totalMb = totalBytes / (1024.0d * 1024.0d);
			var latency = (double) BenchmarkData.Duration.TotalMilliseconds / ((double) BenchmarkData.MessagesClientReceived / 1000.0d);

			sb.AppendLine($"Total data: {totalMb:0.00} MB");
			sb.AppendLine($"Data throughput: {totalMb / BenchmarkData.Duration.TotalSeconds:0.00} MB/s");
			sb.AppendLine($"Message throughput: {BenchmarkData.MessagesClientReceived / BenchmarkData.Duration.TotalSeconds:n0} msg/s");
			sb.AppendLine($"Message latency: {latency:0.000} μs");
			sb.AppendLine("```");
			sb.AppendLine();

			return sb.ToString();
		}
	}
}
