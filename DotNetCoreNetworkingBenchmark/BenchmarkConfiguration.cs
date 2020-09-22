using System;
using System.Text;

namespace DotNetCoreNetworkingBenchmark
{
    internal class BenchmarkConfiguration
    {
	    public readonly BenchmarkData BenchmarkData;
	    public NetworkLibrary Library = NetworkLibrary.NetCoreServer;
        public int Port = 3333;
        public string Address = "127.0.0.1";

        public int NumClients = 1000;
        public int MessageByteSize = 32;
        public byte[] Message { get; private set; }
        public int TestDurationInSeconds = 3;

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
            sb.AppendLine($"Number of Clients: {NumClients}");
            sb.AppendLine($"Message Size: {MessageByteSize} bytes");
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
	        sb.AppendLine($"Number of Clients: {NumClients:n0}");
	        sb.AppendLine($"Message Size: {MessageByteSize:n0} bytes");
	        sb.AppendLine($"Duration: {BenchmarkData.Duration.ToString(@"ss\.fff")} seconds");
	        sb.AppendLine($"Messages Sent Clients: {BenchmarkData.MessagesClientSent:n0}");
	        sb.AppendLine($"Messages Received Server: {BenchmarkData.MessagesServerReceived:n0}");
	        sb.AppendLine($"Messages Sent Server: {BenchmarkData.MessagesServerSent:n0}");
	        sb.AppendLine($"Messages Received Clients: {BenchmarkData.MessagesClientReceived:n0}");

	        var rtt = (double) BenchmarkData.Duration.TotalMilliseconds / ((double) BenchmarkData.MessagesClientReceived / 1000.0d);
	        sb.AppendLine($"Average Round Trip Time: {rtt:0.000} μs");
	        sb.AppendLine(new string('-', header.Length));

	        return sb.ToString();
        }
    }
}
