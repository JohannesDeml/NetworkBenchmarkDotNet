using System;
using System.Text;

namespace NetCoreNetworkBenchmark
{
    internal class BenchmarkConfiguration
    {
	    public readonly BenchmarkData BenchmarkData;
	    public NetworkLibrary Library = NetworkLibrary.NetCoreServer;
        public int Port = 3333;
        public string Address = "127.0.0.1";

        public int NumClients = 1000;
        public int ParallelMessagesPerClient = 100;
        public int MessageByteSize = 32;
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
            var header = "--- Benchmark Configuration ---";
            var sb = new StringBuilder();

            sb.AppendLine(header);
            sb.AppendLine($"Library: {Library}");
            sb.AppendLine($"Address: {Address}, Port: {Port}");
            sb.AppendLine($"Number of clients: {NumClients}");
            sb.AppendLine($"Parallel messages per client: {ParallelMessagesPerClient:n0}");
            sb.AppendLine($"Message size: {MessageByteSize} bytes");
            sb.AppendLine($"Duration: {TestDurationInSeconds} seconds");
            sb.AppendLine(new string('-', header.Length));

            return sb.ToString();
        }

        public string PrintStatistics()
        {
	        var header = "--- Benchmark Results ---";
	        var sb = new StringBuilder();

	        sb.AppendLine();
	        sb.AppendLine(header);
	        sb.AppendLine($"Library: {Library}");
	        sb.AppendLine($"Number of clients: {NumClients:n0}");
	        sb.AppendLine($"Parallel messages per client: {ParallelMessagesPerClient:n0}");
	        sb.AppendLine($"Message size: {MessageByteSize:n0} bytes");
	        sb.AppendLine($"Duration: {BenchmarkData.Duration.TotalSeconds:0.000} s");
	        sb.AppendLine($"Messages sent by clients: {BenchmarkData.MessagesClientSent:n0}");
	        sb.AppendLine($"Messages server received: {BenchmarkData.MessagesServerReceived:n0}");
	        sb.AppendLine($"Messages sent by server: {BenchmarkData.MessagesServerSent:n0}");
	        sb.AppendLine($"Messages clients received: {BenchmarkData.MessagesClientReceived:n0}");

	        var totalBytes = BenchmarkData.MessagesClientReceived * MessageByteSize;
	        var totalMb = totalBytes / (1024.0d * 1024.0d);
	        sb.AppendLine($"Total data: {totalMb:0.00} MB");

	        var latency = (double) BenchmarkData.Duration.TotalMilliseconds / ((double) BenchmarkData.MessagesClientReceived / 1000.0d);
	        sb.AppendLine($"Data throughput: {totalMb/BenchmarkData.Duration.TotalSeconds:0.00} MB/s");
	        sb.AppendLine($"Message throughput: {BenchmarkData.MessagesClientReceived/BenchmarkData.Duration.TotalSeconds:n0} msg/s");
	        sb.AppendLine($"Message latency: {latency:0.000} μs");

	        sb.AppendLine(new string('-', header.Length));

	        return sb.ToString();
        }
    }
}
