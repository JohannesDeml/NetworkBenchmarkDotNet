using System.Text;

namespace DotNetCoreNetworkingBenchmark
{
    internal class BenchmarkConfiguration
    {
        public bool ShowHelp = false;
        public NetworkLibrary Library = NetworkLibrary.ENet;
        public int Port = 3333;
        public string Address = "127.0.0.1";
        
        public int NumClients = 1000;
        public int MessageByteSize = 32;
        public int TestDurationInSeconds = 10;

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
    }
}