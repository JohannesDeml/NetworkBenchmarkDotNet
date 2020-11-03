// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkConfiguration.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Text;

namespace NetCoreNetworkBenchmark
{
	public class BenchmarkConfiguration
	{
		/// <summary>
		/// Run the predefined benchmark
		/// Ignores all other settings below
		/// </summary>
		public bool PredefinedBenchmark { get; set; }

		/// <summary>
		/// Test type that is used in the benchmark
		/// </summary>
		public TestType Test { get; set; }

		/// <summary>
		/// Library used in the benchmark
		/// </summary>
		public NetworkLibrary Library { get; set; }

		/// <summary>
		/// Time the test will run
		/// This time excludes preparations such as clients connecting to the server.
		/// This time excludes cleanup such as clients disconnecting from the server.
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
		/// </summary>
		public int ClientTickRate { get; set; }

		/// <summary>
		/// Number of Updates per second for the server
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

			switch (MessagePayload)
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
					throw new ArgumentOutOfRangeException();
			}
		}

		public string PrintConfiguration()
		{
			var sb = new StringBuilder();

			sb.AppendLine($"### NCNB v{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}");
			sb.AppendLine(
				$"* OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription} {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
			sb.AppendLine($"* Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
			sb.AppendLine($"* Test: {Test} with {Library}");
			sb.AppendLine($"* Address: {Address}, Port: {Port}");
			sb.AppendLine($"* TickRate per second: Client: {ClientTickRate}, Server: {ServerTickRate}");
			sb.AppendLine($"* Number of clients: {Clients}");
			sb.AppendLine($"* Parallel messages per client: {ParallelMessages:n0}");
			sb.AppendLine($"* Message size: {MessageByteSize} bytes");
			sb.AppendLine($"* Message Payload: {MessagePayload}");
			sb.AppendLine($"* Defined duration: {Duration} seconds");
			sb.AppendLine($"* Reproduce: `{CreateCommandlineInstruction()}`");
			sb.AppendLine();

			return sb.ToString();
		}

		public string CreateCommandlineInstruction()
		{
			var sb = new StringBuilder("./NetCoreNetworkBenchmark");

			sb.Append($" -t {Test}");
			sb.Append($" -l {Library}");
			sb.Append($" -a {Address}");
			sb.Append($" -p {Port}");
			sb.Append($" -c {Clients}");
			sb.Append($" -s {MessageByteSize}");
			sb.Append($" -x {MessagePayload}");
			sb.Append($" -m {ParallelMessages}");
			sb.Append($" -d {Duration}");

			return sb.ToString();
		}

		public static void ApplyPredefinedBenchmarkConfiguration(ref BenchmarkConfiguration config)
		{
			config.PredefinedBenchmark = true;
			config.Test = TestType.PingPong;
			config.Address = "127.0.0.1";
			config.Port = 3333;
			config.Duration = 60;
			config.Verbose = false;
			config.MessagePayload = MessagePayload.Random;
			config.MessageByteSize = 32;
			config.ServerTickRate = 60;
			config.ClientTickRate = 60;
		}
	}
}
