// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkCoordinator.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using System.Threading;
using Perfolizer.Horology;

namespace NetworkBenchmark
{
	public static class BenchmarkCoordinator
	{
		public static Configuration Config { get; set; }
		public static readonly BenchmarkStatistics BenchmarkStatistics = new BenchmarkStatistics();

		/// <summary>
		/// Apply the default settings for a predefined benchmark
		/// </summary>
		public static void ApplyPredefinedConfiguration()
		{
			if (Config == null)
			{
				Config = new Configuration();
			}

			Configuration.ApplyPredefinedBenchmarkConfiguration(Config);
		}

		/// <summary>
		/// Prepare the server and clients for the benchmark.
		/// Starts all instances and connects the clients to the server.
		/// </summary>
		/// <param name="networkBenchmark">Library to use for the benchmark</param>
		public static void PrepareBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Prepare Benchmark.");
			Config.PrepareForNewBenchmark();
			networkBenchmark.Initialize(Config, BenchmarkStatistics);
			Utilities.WriteVerbose(".");

			if (Config.IsRunServer())
			{
				var serverTask = networkBenchmark.StartServer();
				serverTask.Wait();
			}

			if (Config.IsRunClients())
			{
				var clientTask = networkBenchmark.StartClients();
				clientTask.Wait();
			}

			Utilities.WriteVerbose(".");

			if (Config.IsRunClients())
			{
				networkBenchmark.ConnectClients().Wait();
			}
			Utilities.WriteVerboseLine(" Done");
		}

		/// <summary>
		/// Run the benchmark for a specific duration
		/// The benchmark needs to be prepared once before running it.
		/// </summary>
		/// <param name="networkBenchmark">Library to run</param>
		public static void RunTimedBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose($"-> Run Benchmark {Config.Library}...");
			StartBenchmark(networkBenchmark);

			Thread.Sleep(Config.Duration * 1000);

			StopBenchmark(networkBenchmark);
			Utilities.WriteVerboseLine(" Done");
		}

		/// <summary>
		/// Runs until the user stops the process
		/// </summary>
		/// <param name="networkBenchmark"></param>
		public static void RunIndefinitely(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose($"-> Run indefinitely {Config.Library}... (press enter to stop)");
			StartBenchmark(networkBenchmark);

			Console.ReadLine();

			StopBenchmark(networkBenchmark);
			Utilities.WriteVerboseLine(" Done");
		}

		public static void StartBenchmark(INetworkBenchmark networkBenchmark)
		{
			BenchmarkStatistics.Reset();
			BenchmarkStatistics.StartBenchmark();
			networkBenchmark.StartBenchmark();
		}

		public static void StopBenchmark(INetworkBenchmark networkBenchmark)
		{
			networkBenchmark.StopBenchmark();
			BenchmarkStatistics.StopBenchmark();
		}

		public static void CleanupBenchmark(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose("-> Clean up.");

			if (Config.IsRunClients())
			{
				networkBenchmark.DisconnectClients().Wait();
				networkBenchmark.StopClients().Wait();
				networkBenchmark.DisposeClients().Wait();
			}
			Utilities.WriteVerbose(".");


			if (Config.IsRunServer())
			{
				networkBenchmark.StopServer().Wait();
			}
			Utilities.WriteVerbose(".");
			if (Config.IsRunServer())
			{
				networkBenchmark.DisposeServer().Wait();
			}

			networkBenchmark.Deinitialize();
			Utilities.WriteVerboseLine(" Done");
			Utilities.WriteVerboseLine("");
		}

		public static string PrintStatistics()
		{
			var sb = new StringBuilder();

			sb.AppendLine("```");
			sb.AppendLine($"Results {Config.Library} with {Config.Transmission} {Config.Test}");
			if (BenchmarkStatistics.Errors > 0)
			{
				sb.AppendLine($"Errors: {BenchmarkStatistics.Errors}");
				sb.AppendLine();
			}

			sb.AppendLine($"Duration: {BenchmarkStatistics.Duration.TotalSeconds:0.000} s");
			sb.AppendLine($"Messages sent by clients: {BenchmarkStatistics.MessagesClientSent:n0}");
			sb.AppendLine($"Messages server received: {BenchmarkStatistics.MessagesServerReceived:n0}");
			sb.AppendLine($"Messages sent by server: {BenchmarkStatistics.MessagesServerSent:n0}");
			sb.AppendLine($"Messages clients received: {BenchmarkStatistics.MessagesClientReceived:n0}");
			sb.AppendLine();

			var totalBytes = BenchmarkStatistics.MessagesClientReceived * Config.MessageByteSize;
			var totalMb = totalBytes / (1024.0d * 1024.0d);
			var latency = new TimeInterval(BenchmarkStatistics.Duration.TotalMilliseconds * Config.Clients / BenchmarkStatistics.MessagesClientReceived,
				TimeUnit.Millisecond);

			sb.AppendLine($"Total data: {totalMb:0.00} MB");
			sb.AppendLine($"Data throughput: {totalMb / BenchmarkStatistics.Duration.TotalSeconds:0.00} MB/s");
			sb.AppendLine($"Message throughput: {BenchmarkStatistics.MessagesClientReceived / BenchmarkStatistics.Duration.TotalSeconds:n0} msg/s");
			sb.AppendLine($"Client Message latency: {latency.ToString()}");
			sb.AppendLine("```");
			sb.AppendLine();

			return sb.ToString();
		}
	}
}
