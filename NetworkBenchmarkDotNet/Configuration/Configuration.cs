// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Text;

namespace NetworkBenchmark
{
	public class Configuration
	{
		/// <summary>
		/// Run the predefined benchmark
		/// Ignores all other settings below
		/// </summary>
		public BenchmarkMode Benchmark { get; set; }

		/// <summary>
		/// Define with parts of the library are run
		/// When testing locally, set this to complete.
		/// </summary>
		public ExecutionMode ExecutionMode { get; set; }

		/// <summary>
		/// Test type that is used in the benchmark
		/// </summary>
		public TestType Test { get; set; }

		/// <summary>
		/// Library used in the benchmark
		/// </summary>
		public NetworkLibrary Library { get; set; }

		/// <summary>
		/// Type of message transmission between server and client and vice versa
		/// </summary>
		public TransmissionType Transmission { get; set; }

		/// <summary>
		/// Time the test will run
		/// This time excludes preparations such as clients connecting to the server.
		/// This time excludes cleanup such as clients disconnecting from the server.
		/// Setting this to a negative value will make the process run infinitely
		/// </summary>
		public int Duration { get; set; }

		/// <summary>
		/// Target address. If you want to test locally its "127.0.0.1" for ipv4 and "::1" for ipv6
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Server port
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Number of clients used for the test
		/// </summary>
		public int Clients { get; set; }

		/// <summary>
		/// Number of messages each client exchanges with the server in parallel
		/// Interesting if you want to see how well messages are merged
		/// </summary>
		public int ParallelMessages { get; set; }

		/// <summary>
		/// Size of each message
		/// </summary>
		public int MessageByteSize { get; set; }

		/// <summary>
		/// Message content, might be interesting for compression or packet sniffing
		/// </summary>
		public MessagePayload MessagePayload { get; set; }

		/// <summary>
		/// Output additional information about current step and errors to the console
		/// </summary>
		public bool Verbose { get; set; }

		/// <summary>
		/// Number of Updates per second for each client
		/// Not supported by all libraries!
		/// </summary>
		public int ClientTickRate { get; set; }

		/// <summary>
		/// Number of Updates per second for the server
		/// Not supported by all libraries!
		/// </summary>
		public int ServerTickRate { get; set; }

		public byte[] Message { get; private set; }

		public void PrepareForNewBenchmark()
		{
			GenerateMessageBytes();
		}

		private void GenerateMessageBytes()
		{
			Message = new byte[MessageByteSize];

			FillWithPayload(MessagePayload);
		}

		private void FillWithPayload(MessagePayload payload)
		{
			switch (payload)
			{
				case MessagePayload.Random:
					var rnd = new Random(0);
					rnd.NextBytes(Message);
					break;
				case MessagePayload.Zeros:
					break;
				case MessagePayload.Ones:
					for (int i = 0; i < MessageByteSize; i++)
					{
						Message[i] = 0xff;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(payload));
			}
		}

		public bool IsRunServer()
		{
			return (ExecutionMode & ExecutionMode.Server) != 0;
		}

		public bool IsRunClients()
		{
			return (ExecutionMode & ExecutionMode.Client) != 0;
		}

		public string ToFormattedString()
		{
			var sb = new StringBuilder();

			AppendEnvironmentSetup(sb);
			sb.AppendLine($"* Running {Library} for {GetDurationString()}");
			sb.AppendLine($"* Test: {Test} with {Transmission} messages");
			sb.AppendLine($"* Address: {Address}, Port: {Port}, Mode: {ExecutionMode}");
			if (IsRunClients())
			{
				sb.AppendLine($"* Number of clients: {Clients}");
			}

			sb.AppendLine($"* Parallel messages: {ParallelMessages:n0}, Size: {MessageByteSize} bytes, Payload: {MessagePayload}");
			sb.AppendLine($"* TickRate per second: Client: {ClientTickRate}, Server: {ServerTickRate}");
			sb.AppendLine($"* Reproduce: `");
			AppendCommandlineInstruction(sb);
			sb.AppendLine($"`");

			sb.AppendLine();

			return sb.ToString();
		}

		private string GetDurationString()
		{
			if (Duration < 0)
			{
				return "indefinite time";
			}

			return $"{Duration} seconds";
		}

		public string ExecutionEnvironmentToString()
		{
			var sb = new StringBuilder();
			AppendEnvironmentSetup(sb);
			return sb.ToString();
		}

		public void AppendEnvironmentSetup(StringBuilder sb)
		{
			sb.AppendLine($"### NBN v{GetType().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version} Benchmark {Benchmark}");
			sb.AppendLine(
				$"* OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription} {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
			sb.AppendLine($"* Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
		}

		public void AppendCommandlineInstruction(StringBuilder sb)
		{
			sb.Append("./NetworkBenchmarkDotNet");
			sb.Append($" --execution-mode {ExecutionMode}");
			sb.Append($" --test {Test}");
			sb.Append($" --transmission {Transmission}");
			sb.Append($" --library {Library}");
			sb.Append($" --duration {Duration}");
			sb.Append($" --address {Address}");
			sb.Append($" --port {Port}");
			sb.Append($" --clients {Clients}");
			sb.Append($" --parallel-messages {ParallelMessages}");
			sb.Append($" --message-byte-size {MessageByteSize}");
			sb.Append($" --message-payload {MessagePayload}");
			sb.Append($" --client-tick-rate {ClientTickRate}");
			sb.Append($" --server-tick-rate {ServerTickRate}");
		}

		public static void ApplyPredefinedBenchmarkConfiguration(Configuration config)
		{
			config.ExecutionMode = ExecutionMode.Complete;
			config.Test = TestType.PingPong;
			config.Transmission = TransmissionType.Unreliable;
			config.Address = "::1";
			config.Port = 3330;
			config.Duration = 60;
			config.Verbose = false;
			config.MessagePayload = MessagePayload.Random;
			config.MessageByteSize = 32;
			config.ServerTickRate = 60;
			config.ClientTickRate = 60;
		}
	}
}
