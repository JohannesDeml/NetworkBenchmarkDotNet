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
		public TestType TestType = TestType.PingPong;
		public NetworkLibrary Library = NetworkLibrary.ElfhildNet;
		public int Port = 3333;

		/// <summary>
		/// Target address. If you want to test locally its "127.0.0.1" for ipv4 and "::1" for ipv6
		/// </summary>
		public string Address = "127.0.0.1";

		public bool Verbose = true;

		public string Name = "Custom";

		public int NumClients = 1;
		public int ParallelMessagesPerClient = 1;
		public int MessageByteSize = 32;
		public MessagePayload MessagePayload = MessagePayload.Ones;

		public int TickRateClient = 60;
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

			sb.AppendLine($"### Benchmark {Name} (v {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version})");
			sb.AppendLine($"* `{CreateCommandlineInstruction()}`");
			sb.AppendLine(
				$"* OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription} {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
			sb.AppendLine($"* Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
			sb.AppendLine($"* Test: {TestType}");
			sb.AppendLine($"* Address: {Address}, Port: {Port}");
			sb.AppendLine($"* Number of clients: {NumClients}");
			sb.AppendLine($"* Parallel messages per client: {ParallelMessagesPerClient:n0}");
			sb.AppendLine($"* Message size: {MessageByteSize} bytes");
			sb.AppendLine($"* Message Payload: {MessagePayload}");
			sb.AppendLine($"* Defined duration: {TestDurationInSeconds} seconds");
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
