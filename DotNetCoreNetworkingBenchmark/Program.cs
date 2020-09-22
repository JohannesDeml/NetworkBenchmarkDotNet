using System;
using System.Threading;
using System.Threading.Tasks;
using NDesk.Options;

namespace DotNetCoreNetworkingBenchmark
{
    internal enum NetworkLibrary
    {
        ENet,
        NetCoreServer,
        LiteNetLib
    }

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
                { "h|?|help", "Show help",   v =>  showHelp = (v != null) },
                { "l|library=", "Library target (Default: ENet)", v => Config.Library = (NetworkLibrary)Enum.Parse(typeof(NetworkLibrary), v, true) },
                { "a|address=", "Address to use (Default: 127.0.0.1)", v => Config.Address = v },
                { "p|port=", "Socket Port (Default: 3333)", v => Config.Port = int.Parse(v) },
                { "c|clients=", "Number of simultaneous clients (Default: 1000)", v => Config.NumClients = int.Parse(v) },
                { "s|size=", "Message byte size sent by clients (Default: 20)", v => Config.MessageByteSize = int.Parse(v) },
                { "d|duration=", "Duration fo the test in seconds (Default: 10)", v => Config.TestDurationInSeconds = int.Parse(v) }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("Command line error: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' to get usage information.");
                return;
            }

            if (showHelp)
            {
                Console.WriteLine("Usage:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }


            Console.WriteLine(Config.PrintConfiguration());

            _library = INetworkingLibrary.CreateNetworkingLibrary(Config.Library);

            Console.WriteLine("-> Prepare Benchmark");
            PrepareBenchmark();
            Console.WriteLine("-> Run Benchmark");
            RunBenchmark();
            Console.WriteLine("-> Benchmark Finished - cleaning up");
            CleanupBenchmark();
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
	        var timestampStart = DateTime.UtcNow;
	        Config.BenchmarkData.StartBenchmark();
	        _library.StartBenchmark();
	        Thread.Sleep(Config.TestDurationInSeconds * 1000);
	        Config.BenchmarkData.StopBenchmark();
	        _library.StopBenchmark();
        }

        private static void CleanupBenchmark()
        {
	        _library.DisconnectClients();
	        _library.StopClients();
	        _library.StopServer();
        }

        private static void ShowStatistics()
        {
	        Console.WriteLine(Config.PrintStatistics());
        }
    }
}
