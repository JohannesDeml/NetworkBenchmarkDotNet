using System;
using System.Threading;
using System.Threading.Tasks;
using NDesk.Options;

namespace NetCoreNetworkBenchmark
{
    class Program
    {
        public static BenchmarkConfiguration Config;
        private static INetworkingLibrary _library;

        static void Main(string[] args)
        {
            Config = new BenchmarkConfiguration();
            var showHelp = false;

            var options = new OptionSet()
            {
                { "h|?|help", "Show help",
	                v =>  showHelp = (v != null) },
                { "t|test=", $"Test (Default: {Config.TestType})\nOptions: {Utilities.EnumToString<TestType>()}",
	                v => Config.TestType = Utilities.ParseEnum<TestType>(v) },
                { "l|library=", $"Library target (Default: {Config.Library})\nOptions: {Utilities.EnumToString<NetworkLibrary>()}",
	                v => Config.Library = Utilities.ParseEnum<NetworkLibrary>(v) },
                { "a|address=", $"Address to use (Default: {Config.Address})",
	                v => Config.Address = v },
                { "p|port=", $"Port (Default: {Config.Port})",
	                v => Config.Port = int.Parse(v) },
                { "c|clients=", $"# Simultaneous clients (Default: {Config.NumClients})",
	                v => Config.NumClients = int.Parse(v) },
                { "m|messages=", $"# Parallel messages per client (Default: {Config.ParallelMessagesPerClient})",
	                v => Config.ParallelMessagesPerClient = int.Parse(v) },
                { "s|size=", $"Message byte size sent by clients (Default: {Config.MessageByteSize})",
	                v => Config.MessageByteSize = int.Parse(v) },
                { "d|duration=", $"Duration fo the test in seconds (Default: {Config.TestDurationInSeconds})",
	                v => Config.TestDurationInSeconds = int.Parse(v) }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine($"Command line error: {e.Message}");
                Console.WriteLine("Try `--help' to get usage information.");
                return;
            }

            if (showHelp)
            {
                Console.WriteLine("Usage:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }


            Console.Write(Config.PrintConfiguration());

            _library = INetworkingLibrary.CreateNetworkingLibrary(Config.Library);

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
	        _library.Initialize(Config);

	        var serverTask =  _library.StartServer();
	        var clientTask =  _library.StartClients();
	        await serverTask;
	        await clientTask;

	        await _library.ConnectClients();
        }

        private static void RunBenchmark()
        {
	        Config.BenchmarkData.StartBenchmark();
	        _library.StartBenchmark();
	        Thread.Sleep(Config.TestDurationInSeconds * 1000);
	        _library.StopBenchmark();
	        Config.BenchmarkData.StopBenchmark();
        }

        private static async void CleanupBenchmark()
        {
	        await _library.DisconnectClients();
	        await _library.StopClients();
	        await _library.StopServer();
	        await _library.Dispose();
        }

        private static void ShowStatistics()
        {
	        Console.Write(Config.PrintStatistics());
        }
    }
}
