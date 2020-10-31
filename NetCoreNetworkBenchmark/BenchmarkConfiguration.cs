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
	public enum MessagePayload
	{
		Random,
		Zeros,
		Ones
	}

	public class BenchmarkConfiguration
	{
		/// <summary>
		/// Target address. If you want to test locally its "127.0.0.1" for ipv4 and "::1" for ipv6
		/// </summary>
		public string Address = "127.0.0.1";

		/// <summary>
		/// Server port
		/// </summary>
		public int Port = 3333;

		/// <summary>
		/// Test type that is used in the benchmark
		/// </summary>
		public TestType TestType = TestType.PingPong;

		/// <summary>
		/// Library used in the benchmark
		/// </summary>
		public NetworkLibrary Library = NetworkLibrary.ENet;

		/// <summary>
		/// Output additional information about current step and errors to the console
		/// </summary>
		public bool Verbose = true;

		/// <summary>
		/// Number of clients used for the test
		/// </summary>
		public int NumClients = 1000;

		/// <summary>
		/// Number of messages each client exchanges with the server in parallel
		/// Interesting if you want to see how well messages are merged
		/// </summary>
		public int ParallelMessagesPerClient = 1;

		/// <summary>
		/// Size of each message
		/// </summary>
		public int MessageByteSize = 32;

		/// <summary>
		/// Message content, might be interesting for compression or packet sniffing
		/// </summary>
		public MessagePayload MessagePayload = MessagePayload.Ones;

		/// <summary>
		/// Number of Updates per second for each client
		/// </summary>
		public int TickRateClient = 60;

		/// <summary>
		/// Number of Updates per second for the server
		/// </summary>
		public int TickRateServer = 60;

		public byte[] Message { get; private set; }
		public int TestDurationInSeconds = 10;

		public BenchmarkConfiguration()
		{
		}

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
			sb.AppendLine($"* Test: {TestType} with {Library}");
			sb.AppendLine($"* Address: {Address}, Port: {Port}");
			sb.AppendLine($"* TickRate per second: Client: {TickRateClient}, Server: {TickRateServer}");
			sb.AppendLine($"* Number of clients: {NumClients}");
			sb.AppendLine($"* Parallel messages per client: {ParallelMessagesPerClient:n0}");
			sb.AppendLine($"* Message size: {MessageByteSize} bytes");
			sb.AppendLine($"* Message Payload: {MessagePayload}");
			sb.AppendLine($"* Defined duration: {TestDurationInSeconds} seconds");
			sb.AppendLine($"* Reproduce: `{CreateCommandlineInstruction()}`");
			sb.AppendLine();

			return sb.ToString();
		}

		public string CreateCommandlineInstruction()
		{
			var sb = new StringBuilder("./NetCoreNetworkBenchmark");

			sb.Append($" -t {TestType}");
			sb.Append($" -l {Library}");
			sb.Append($" -a {Address}");
			sb.Append($" -p {Port}");
			sb.Append($" -c {NumClients}");
			sb.Append($" -s {MessageByteSize}");
			sb.Append($" -x {MessagePayload}");
			sb.Append($" -m {ParallelMessagesPerClient}");
			sb.Append($" -d {TestDurationInSeconds}");

			return sb.ToString();
		}
	}
}
