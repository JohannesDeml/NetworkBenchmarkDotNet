using System;
using System.Threading;
using System.Threading.Tasks;
using NDesk.Options;

namespace NetCoreNetworkBenchmark
{
    class Program
    {
        public static BenchmarkConfiguration Config;
        private static INetworkBenchmark _networkBenchmark;

        static void Main(string[] args)
        {
            Config = new BenchmarkConfiguration();
            var showHelp = false;

            var options = new OptionSet()
            {
                { "h|?|help", "Show help",
	                v =>  showHelp = (v != null) },
                { "t|test=", $"Test (Default: {Config.TestType})\nOptions: {Utilities.EnumToString<TestType>()}",
	                v => Utilities.ParseOption(v, out Config.TestType) },
                { "l|library=", $"Library target (Default: {Config.Library})\nOptions: {Utilities.EnumToString<NetworkLibrary>()}",
	                v => Utilities.ParseOption(v, out Config.Library) },
                { "a|address=", $"Address to use (Default: {Config.Address})",
	                v => Config.Address = v },
                { "p|port=", $"Port (Default: {Config.Port})",
	                v => Utilities.ParseOption(v, out Config.Port, 0, 65535) },
                { "c|clients=", $"# Simultaneous clients (Default: {Config.NumClients})",
	                v => Utilities.ParseOption(v, out Config.NumClients, 1, 1024 * 1024) },
                { "m|messages=", $"# Parallel messages per client (Default: {Config.ParallelMessagesPerClient})",
	                v => Utilities.ParseOption(v, out Config.ParallelMessagesPerClient, 1, 1024 * 1024) },
                { "s|size=", $"Message byte size sent by clients (Default: {Config.MessageByteSize})",
	                v => Utilities.ParseOption(v, out Config.MessageByteSize, 1, 1024 * 1024) },
                { "d|duration=", $"Duration fo the test in seconds (Default: {Config.TestDurationInSeconds})",
	                v => Utilities.ParseOption(v, out Config.TestDurationInSeconds, 1) }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine($"Error when parsing options\n{e.Message}\n");
                showHelp = true;
            }

            if (showHelp)
            {
                Console.WriteLine("Usage:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }


            Console.Write(Config.PrintConfiguration());

            _networkBenchmark = INetworkBenchmark.CreateNetworkBenchmark(Config.Library);

            Console.Write("-> Prepare Benchmark...");
            PrepareBenchmark();
            Console.WriteLine(" Done");
            Console.Write("-> Run Benchmark...");
            RunBenchmark();
            Console.WriteLine(" Done");
            Console.Write("-> Clean up...");
            CleanupBenchmark();
            Console.WriteLine(" Done");
            ShowStatistics();
        }

        private static async void PrepareBenchmark()
        {
	        Config.PrepareForNewBenchmark();
	        _networkBenchmark.Initialize(Config);

	        var serverTask =  _networkBenchmark.StartServer();
	        var clientTask =  _networkBenchmark.StartClients();
	        await serverTask;
	        await clientTask;

	        await _networkBenchmark.ConnectClients();
        }

        private static void RunBenchmark()
        {
	        Config.BenchmarkData.StartBenchmark();
	        _networkBenchmark.StartBenchmark();
	        Thread.Sleep(Config.TestDurationInSeconds * 1000);
	        _networkBenchmark.StopBenchmark();
	        Config.BenchmarkData.StopBenchmark();
        }

        private static async void CleanupBenchmark()
        {
	        await _networkBenchmark.DisconnectClients();
	        await _networkBenchmark.StopClients();
	        await _networkBenchmark.StopServer();
	        await _networkBenchmark.Dispose();
        }

        private static void ShowStatistics()
        {
	        Console.Write(Config.PrintStatistics());
        }
    }
}
