using System;
using System.Reflection;
using System.Text;

namespace NetCoreNetworkBenchmark
{
	internal enum NetworkLibrary
	{
		ENet,
		NetCoreServer,
		LiteNetLib
	}

	internal enum TestType
	{
		PingPong
	}

	internal class BenchmarkConfiguration
    {
	    public readonly BenchmarkData BenchmarkData;
	    public TestType TestType = TestType.PingPong;
	    public NetworkLibrary Library = NetworkLibrary.NetCoreServer;
        public int Port = 3333;
        public string Address = "127.0.0.1";
        public bool PrintSteps = true;

        public int NumClients = 1000;
        public int ParallelMessagesPerClient = 100;
        public int MessageByteSize = 32;
        /// <summary>
        /// Tick Rate for fetching events if supported by the library
        /// </summary>
        public int TickRateClient = 100;
        public int TickRateServer = 30;
        public byte[] Message { get; private set; }
        public int TestDurationInSeconds = 10;

        public BenchmarkConfiguration()
        {
	        BenchmarkData = new BenchmarkData();
        }

        public void PrepareForNewBenchmark()
        {
	        GenerateMessageBytes();
	        BenchmarkData.Reset();
	        BenchmarkData.StartBenchmark();
        }

        private void GenerateMessageBytes()
        {
	        var rnd = new Random(0);
	        Message = new byte[MessageByteSize];
	        rnd.NextBytes(Message);
        }

        public string PrintConfiguration()
        {
	        var sb = new StringBuilder();

            sb.AppendLine($"### Benchmark Configuration (v {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version})");
            sb.AppendLine($"OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription} {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
            sb.AppendLine($"Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
            sb.AppendLine($"Test: {TestType}");
            sb.AppendLine($"Library: {Library}");
            sb.AppendLine($"Address: {Address}, Port: {Port}");
            sb.AppendLine($"Number of clients: {NumClients}");
            sb.AppendLine($"Parallel messages per client: {ParallelMessagesPerClient:n0}");
            sb.AppendLine($"Message size: {MessageByteSize} bytes");
            sb.AppendLine($"Defined duration: {TestDurationInSeconds} seconds");

            return sb.ToString();
        }

        public string PrintStatistics()
        {
	        var sb = new StringBuilder();

	        if (BenchmarkData.Errors > 0)
	        {
		        sb.AppendLine();
		        sb.AppendLine($"Errors: {BenchmarkData.Errors}");
	        }

	        sb.AppendLine();
	        sb.AppendLine($"#### {Library}");
	        sb.AppendLine("```");
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
	        sb.AppendLine($"Data throughput: {totalMb/BenchmarkData.Duration.TotalSeconds:0.00} MB/s");
	        sb.AppendLine($"Message throughput: {BenchmarkData.MessagesClientReceived/BenchmarkData.Duration.TotalSeconds:n0} msg/s");
	        sb.AppendLine($"Message latency: {latency:0.000} μs");
	        sb.AppendLine("```");

	        return sb.ToString();
        }
    }
}
