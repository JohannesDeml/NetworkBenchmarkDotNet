// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineUtilities.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.CommandLine;

namespace NetworkBenchmark
{
	public static class CommandLineUtilities
	{
		public static RootCommand GenerateRootCommand()
		{
			var rootCommand = new RootCommand
			{
				new Option<BenchmarkMode>(
					new[] {"--benchmark", "-b"},
					getDefaultValue: () => BenchmarkMode.Custom,
					"Run predefined benchmarks"),
				new Option<ExecutionMode>(
					new[] {"--execution-mode", "-m"},
					getDefaultValue: () => ExecutionMode.Complete,
					"Control what parts to run"),
				new Option<TestType>(
					new[] {"--test", "-t"},
					getDefaultValue: () => TestType.PingPong,
					"Test type"),
				new Option<TransmissionType>(
					new[] {"--transmission"},
					getDefaultValue: () => TransmissionType.Unreliable,
					"Transmission type"),
				new Option<NetworkLibrary>(
					new[] {"--library", "-l"},
					getDefaultValue: () => NetworkLibrary.ENet,
					"Library target"),
				new Option<int>(
					new[] {"--duration", "-d"},
					getDefaultValue: () => 10,
					"Test duration in seconds (-1 for manual stopping)"),
				new Option<string>(
					"--address",
					getDefaultValue: () => "::1",
					"IP Address, can be ipv4 (e.g. 127.0.0.1) or ipv6 (e.g. ::1)"),
				new Option<int>(
					"--port",
					getDefaultValue: () => 3330,
					"Socket Port"),
				new Option<int>(
					"--clients",
					getDefaultValue: () => 500,
					"# Simultaneous clients"),
				new Option<int>(
					"--parallel-messages",
					getDefaultValue: () => 1,
					"# Parallel messages per client"),
				new Option<int>(
					"--message-byte-size",
					getDefaultValue: () => 32,
					"Message byte size sent by clients"),
				new Option<MessagePayload>(
					"--message-payload",
					getDefaultValue: () => MessagePayload.Random,
					"Message load sent by clients"),
				new Option<bool>(
					"--verbose",
					getDefaultValue: () => true,
					"Verbose output of test steps and errors"),
				new Option<int>(
					"--client-tick-rate",
					getDefaultValue: () => 60,
					"Client ticks per second if supported"),
				new Option<int>(
					"--server-tick-rate",
					getDefaultValue: () => 60,
					"Server ticks per second if supported")
			};

			rootCommand.Name = "NetworkBenchmarkDotNet";
			rootCommand.Description = "Benchmark Low Level .Net Core Networking libraries for UDP socket performance";

			return rootCommand;
		}
	}
}
