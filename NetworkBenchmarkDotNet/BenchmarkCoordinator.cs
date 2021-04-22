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
using System.Threading;

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

		public static void RunBenchmark(INetworkBenchmark networkBenchmark)
		{
			if (Config.Test == TestType.Manual)
			{
				RunManualMode(networkBenchmark);
				return;
			}

			if (Config.Duration < 0)
			{
				RunIndefinitely(networkBenchmark);
				return;
			}

			RunTimedBenchmark(networkBenchmark);
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

		/// <summary>
		/// Enables to enter defined commands
		/// Runs until the user stops the process
		/// </summary>
		/// <param name="networkBenchmark"></param>
		public static void RunManualMode(INetworkBenchmark networkBenchmark)
		{
			Utilities.WriteVerbose($"-> Run Manual Mode {Config.Library}\n");
			StartBenchmark(networkBenchmark);

			while (true)
			{
				var input = Console.ReadLine();
				if (input == null)
				{
					PrintInvalidManualInput();
					continue;
				}
				var parts = input.ToLower().Split(' ');
				if (parts.Length == 0 || parts[0].Length == 0)
				{
					PrintInvalidManualInput();
					continue;
				}

				var target = parts[0][0];

				if (target == 'q')
				{
					break;
				}

				if (parts.Length < 2 || !int.TryParse(parts[1], out int value))
				{
					PrintInvalidManualInput();
					continue;
				}

				switch (target)
				{
					case 'c':
						var clients = networkBenchmark.Clients;
						for (int i = 0; i < clients.Count; i++)
						{
							clients[i].SendMessages(value);
						}

						break;
					case 's':
						networkBenchmark.Server.SendMessages(value);
						break;
					default:
						PrintInvalidManualInput();
						break;
				}
			}

			StopBenchmark(networkBenchmark);
			Utilities.WriteVerboseLine(" Done");
		}

		private static void PrintInvalidManualInput()
		{
			Utilities.WriteVerbose($"Invalid - Enter a command, e.g. c 1 or s 4, to quit enter q");
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
			return BenchmarkStatistics.PrintStatistics(Config);
		}
	}
}
