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
	public enum NetworkLibrary
	{
		ENet,
		NetCoreServer,
		LiteNetLib
	}

	public enum TestType
	{
		PingPong
	}

	public enum MessagePayload
	{
		Random,
		Zeros,
		Ones
	}

	public class BenchmarkConfiguration
	{
		public readonly BenchmarkData BenchmarkData;
		public TestType TestType = TestType.PingPong;
		public NetworkLibrary Library = NetworkLibrary.ENet;
		public int Port = 3333;

		/// <summary>
		/// Target address. If you want to test locally its "127.0.0.1" for ipv4 and "::1" for ipv6
		/// </summary>
		public string Address = "127.0.0.1";

		public bool Verbose = true;

		public string Name = "Custom";

		public int NumClients = 1000;
		public int ParallelMessagesPerClient = 1;
		public int MessageByteSize = 32;
		public MessagePayload MessagePayload = MessagePayload.Ones;

		public int TickRateClient = 60;
		public int TickRateServer = 60;

		public byte[] Message { get; private set; }
		public int TestDurationInSeconds = 10;

		public BenchmarkConfiguration()
		{
			BenchmarkData = new BenchmarkData();
		}

		public void PrepareForNewBenchmark()
		{
			GenerateMessageBytes();
			BenchmarkData.PrepareBenchmark();
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

		public string PrintStatistics()
		{
			var sb = new StringBuilder();

			sb.AppendLine($"#### {Library}");
			sb.AppendLine("```");
			if (BenchmarkData.Errors > 0)
			{
				sb.AppendLine($"Errors: {BenchmarkData.Errors}");
				sb.AppendLine();
			}

			sb.AppendLine($"Duration: {BenchmarkData.Duration.TotalSeconds:0.000} s");
			sb.AppendLine($"Messages sent by clients: {BenchmarkData.MessagesClientSent:n0}");
			sb.AppendLine($"Messages server received: {BenchmarkData.MessagesServerReceived:n0}");
			sb.AppendLine($"Messages sent by server: {BenchmarkData.MessagesServerSent:n0}");
			sb.AppendLine($"Messages clients received: {BenchmarkData.MessagesClientReceived:n0}");
			sb.AppendLine();

			var totalBytes = BenchmarkData.MessagesClientReceived * MessageByteSize;
			var totalMb = totalBytes / (1024.0d * 1024.0d);
			var latency = (double) BenchmarkData.Duration.TotalMilliseconds / ((double) BenchmarkData.MessagesClientReceived / 1000.0d);

			sb.AppendLine($"Total data: {totalMb:0.00} MB");
			sb.AppendLine($"Data throughput: {totalMb / BenchmarkData.Duration.TotalSeconds:0.00} MB/s");
			sb.AppendLine($"Message throughput: {BenchmarkData.MessagesClientReceived / BenchmarkData.Duration.TotalSeconds:n0} msg/s");
			sb.AppendLine($"Message latency: {latency:0.000} μs");
			sb.AppendLine("```");
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
