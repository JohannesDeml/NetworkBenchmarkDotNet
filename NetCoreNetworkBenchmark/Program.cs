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
using BenchmarkDotNet.Running;

namespace NetCoreNetworkBenchmark
{
	class Program
	{
		static int Main(string[] args)
		{
			var rootCommand = new RootCommand
			{
				new Option<bool>(
					new [] {"--predefined-benchmark", "-b"},
					getDefaultValue:() => false,
					"Run predefined full benchmark with all tests and libraries, ignores all other settings"),
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
					getDefaultValue:() => "127.0.0.1",
					"IP Address, can be ipv4 or ipv6"),
				new Option<int>(
					"--port",
					getDefaultValue:() => 3333,
					"Socket Port"),
				new Option<int>(
					"--clients",
					getDefaultValue:() => 1000,
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
					"Number of ticks per second for clients, for libraries that use ticks"),
				new Option<int>(
					"--server-tick-rate",
					getDefaultValue:() => 60,
					"Number of ticks per second for the server, for libraries that use ticks")

			};

			rootCommand.Name = "NetCoreNetworkBenchmark";
			rootCommand.Description = "Benchmark Low Level .Net Core Networking libraries for UDP socket performance";

			rootCommand.Handler = CommandHandler.Create<BenchmarkConfiguration>((config) =>
			{
				Benchmark.Config = config;
				if (config.PredefinedBenchmark)
				{
					BenchmarkRunner.Run<PredefinedBenchmark>();
					return 0;
				}

				Console.Write(config.PrintConfiguration());
				Run();
				return 0;
			});

			return rootCommand.Invoke(args);
		}

		private static void Run()
		{
			var networkBenchmark = INetworkBenchmark.CreateNetworkBenchmark(Benchmark.Config.Library);

			try
			{
				Benchmark.PrepareBenchmark(networkBenchmark);
				Benchmark.RunTimedBenchmark(networkBenchmark);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error when running Library {Benchmark.Config.Library}" +
				                  $"\n{e.Message}\n{e.StackTrace}");
			}
			finally
			{
				try
				{
					Benchmark.CleanupBenchmark(networkBenchmark);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error when cleaning up Library {Benchmark.Config.Library}" +
					                  $"\n{e.Message}\n{e.StackTrace}");
				}

				Console.Write(Benchmark.PrintStatistics());
			}
		}
	}
}
