// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NetworkBenchmark
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			RootCommand rootCommand = CommandLineUtilities.GenerateRootCommand();

			rootCommand.Handler = CommandHandler.Create<Configuration>((config) =>
			{
				BenchmarkCoordinator.Config = config;
				var mode = config.Benchmark;


				if (mode == BenchmarkMode.Custom)
				{
					Console.Write(config.ToFormattedString());
					RunCustomBenchmark();
					return 0;
				}

				Console.Write(config.ExecutionEnvironmentToString());
				RunPredefinedBenchmarks(mode);
				return 0;
			});

			return rootCommand.Invoke(args);
		}

		private static void RunPredefinedBenchmarks(BenchmarkMode mode)
		{
			if ((mode & BenchmarkMode.Performance) != 0)
			{
				RunBenchmark<UnreliablePerformanceBenchmark>();
				RunBenchmark<ReliablePerformanceBenchmark>();
				Console.WriteLine($"Finished {BenchmarkMode.Performance} Benchmark");
			}

			if ((mode & BenchmarkMode.Quick) != 0)
			{
				RunBenchmark<QuickBenchmark>();
				Console.WriteLine($"Finished {BenchmarkMode.Quick} Benchmark");
			}

			if ((mode & BenchmarkMode.Sampling) != 0)
			{
				RunBenchmark<SamplingBenchmark>();
				Console.WriteLine($"Finished {BenchmarkMode.Sampling} Benchmark");
			}
		}

		private static void RunCustomBenchmark()
		{
			var networkBenchmark = INetworkBenchmark.CreateNetworkBenchmark(BenchmarkCoordinator.Config.Library);

			try
			{
				BenchmarkCoordinator.PrepareBenchmark(networkBenchmark);
				BenchmarkCoordinator.RunBenchmark(networkBenchmark);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error when running Library {BenchmarkCoordinator.Config.Library}" +
				                  $"\n{e.Message}\n{e.StackTrace}");
			}
			finally
			{
				try
				{
					BenchmarkCoordinator.CleanupBenchmark(networkBenchmark);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error when cleaning up Library {BenchmarkCoordinator.Config.Library}" +
					                  $"\n{e.Message}\n{e.StackTrace}");
				}

				Console.Write(BenchmarkCoordinator.PrintStatistics());
			}
		}

		/// <summary>
		/// Run a benchmark and throw an error is something didn't succeed.
		/// Useful for CI, where the benchmark should stop if something does not work as expected.
		/// </summary>
		/// <typeparam name="T">Type of the benchmark to run</typeparam>
		private static void RunBenchmark<T>()
		{
			ManualConfig config = ManualConfig.CreateMinimumViable();
			var summary = BenchmarkRunner.Run<T>(config);

			Assert(!summary.HasCriticalValidationErrors, "The \"Summary\" should have NOT \"HasCriticalValidationErrors\"");

			Assert(summary.Reports.Any(), "The \"Summary\" should contain at least one \"BenchmarkReport\" in the \"Reports\" collection");

			Assert(summary.Reports.All(r => r.BuildResult.IsBuildSuccess),
				"The following benchmarks are failed to build: " +
				string.Join(", ", summary.Reports.Where(r => !r.BuildResult.IsBuildSuccess).Select(r => r.BenchmarkCase.DisplayInfo)));

			Assert(summary.Reports.All(r => r.ExecuteResults.Any(er => er.FoundExecutable && er.Results.Any())),
				"All reports should have at least one \"ExecuteResult\" with \"FoundExecutable\" = true and at least one \"Data\" item");

			Assert(summary.Reports.All(report => report.AllMeasurements.Any()),
				"All reports should have at least one \"Measurement\" in the \"AllMeasurements\" collection");
		}

		private static void Assert(bool assertTrue, string message)
		{
			if (!assertTrue)
			{
				throw new Exception("Assertion exception: " + message);
			}
		}
	}
}
