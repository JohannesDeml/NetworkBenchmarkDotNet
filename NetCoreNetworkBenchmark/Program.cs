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
using System.Threading;
using BenchmarkDotNet.Running;
using NDesk.Options;

namespace NetCoreNetworkBenchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = Benchmark.Config;
			var showHelp = false;
			var predefinedBenchmark = false;

			var options = new OptionSet()
			{
				{
					"h|?|help", "Show help",
					v => showHelp = (v != null)
				},
				{
					"b|benchmark", "Run predefined full benchmark with all tests and libraries, ignores all other settings",
					v => predefinedBenchmark = (v != null)
				},
				{
					"t|test=", $"Test (Default: {config.TestType})\nOptions: {Utilities.EnumToString<TestType>()}",
					v => Utilities.ParseOption(v, out config.TestType)
				},
				{
					"l|library=", $"Library target (Default: {config.Library})\nOptions: {Utilities.EnumToString<NetworkLibrary>()}",
					v => Utilities.ParseOption(v, out config.Library)
				},
				{
					"a|address=", $"Address to use (Default: {config.Address})",
					v => config.Address = v
				},
				{
					"p|port=", $"Port (Default: {config.Port})",
					v => Utilities.ParseOption(v, out config.Port, 0, 65535)
				},
				{
					"c|clients=", $"# Simultaneous clients (Default: {config.NumClients})",
					v => Utilities.ParseOption(v, out config.NumClients, 1, 1024 * 1024)
				},
				{
					"m|messages=", $"# Parallel messages per client (Default: {config.ParallelMessagesPerClient})",
					v => Utilities.ParseOption(v, out config.ParallelMessagesPerClient, 1, 1024 * 1024)
				},
				{
					"s|size=", $"Message byte size sent by clients (Default: {config.MessageByteSize})",
					v => Utilities.ParseOption(v, out config.MessageByteSize, 1, 1024 * 1024)
				},
				{
					"x|messageload=", $"Message load sent by clients (Default: {config.MessagePayload})\nOptions: {Utilities.EnumToString<MessagePayload>()}",
					v => Utilities.ParseOption(v, out config.MessagePayload)
				},
				{
					"d|duration=", $"Duration fo the test in seconds (Default: {config.TestDurationInSeconds})",
					v => Utilities.ParseOption(v, out config.TestDurationInSeconds, 1)
				}
			};

			try
			{
				options.Parse(args);
			}
			catch (OptionException e)
			{
				Console.WriteLine($"Error when parsing options\n{e.Message}\n");
				showHelp = true;
			}

			if (showHelp)
			{
				Console.WriteLine("Usage:");
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			if (predefinedBenchmark)
			{
				BenchmarkRunner.Run<PredefinedBenchmark>();
				//RunPredefinedBenchmark();
				return;
			}

			Console.Write(config.PrintConfiguration());
			Run();
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
