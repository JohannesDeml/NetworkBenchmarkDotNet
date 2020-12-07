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
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NetworkBenchmark
{
	class Program
	{
		static int Main(string[] args)
		{
			var rootCommand = new RootCommand
			{
				new Option<BenchmarkMode>(
					new [] {"--benchmark", "-b"},
					getDefaultValue:() => BenchmarkMode.Custom,
					"Run predefined benchmarks"),
				new Option<TestType>(
					new [] {"--test", "-t"},
					getDefaultValue:() => TestType.PingPong,
					"Test type"),
				new Option<NetworkLibrary>(
					new [] {"--library", "-l"},
					getDefaultValue:() => NetworkLibrary.ENet,
					"Library target"),
				new Option<int>(
					new [] {"--duration", "-d"},
					getDefaultValue:() => 10,
					"Test duration in seconds"),
				new Option<string>(
					"--address",
					getDefaultValue:() => "::1",
					"IP Address, can be ipv4 (e.g. 127.0.0.1) or ipv6 (e.g. ::1)"),
				new Option<int>(
					"--port",
					getDefaultValue:() => 3330,
					"Socket Port"),
				new Option<int>(
					"--clients",
					getDefaultValue:() => 500,
					"# Simultaneous clients"),
				new Option<int>(
					"--parallel-messages",
					getDefaultValue:() => 1,
					"# Parallel messages per client"),
				new Option<int>(
					"--message-byte-size",
					getDefaultValue:() => 32,
					"Message byte size sent by clients"),
				new Option<MessagePayload>(
					"--message-payload",
					getDefaultValue:() => MessagePayload.Random,
					"Message load sent by clients"),
				new Option<bool>(
					"--verbose",
					getDefaultValue:() => true,
					"Verbose output of test steps and errors"),
				new Option<int>(
					"--client-tick-rate",
					getDefaultValue:() => 60,
					"Client ticks per second if supported"),
				new Option<int>(
					"--server-tick-rate",
					getDefaultValue:() => 60,
					"Server ticks per second if supported")

			};

			rootCommand.Name = "NetworkBenchmarkDotNet";
			rootCommand.Description = "Benchmark Low Level .Net Core Networking libraries for UDP socket performance";

			rootCommand.Handler = CommandHandler.Create<BenchmarkSetup>((config) =>
			{
				BenchmarkCoordinator.Config = config;
				var mode = config.Benchmark;


				if (mode == BenchmarkMode.Custom)
				{
					Console.Write(config.PrintSetup());
					Run();
					return 0;
				}

				Console.Write(config.PrintEnvironment());
				if ((mode & BenchmarkMode.Performance) != 0)
				{
					BenchmarkRunner.Run<PerformanceBenchmark>();
					Console.WriteLine($"Finished {BenchmarkMode.Performance} Benchmark");
				}

				if ((mode & BenchmarkMode.Garbage) != 0)
				{
					BenchmarkRunner.Run<GarbageBenchmark>();
					Console.WriteLine($"Finished {BenchmarkMode.Garbage} Benchmark");
				}

				return 0;
			});

			return rootCommand.Invoke(args);
		}

		private static void Run()
		{
			var networkBenchmark = INetworkBenchmark.CreateNetworkBenchmark(BenchmarkCoordinator.Config.Library);

			try
			{
				BenchmarkCoordinator.PrepareBenchmark(networkBenchmark);
				BenchmarkCoordinator.RunTimedBenchmark(networkBenchmark);
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
	}
}
