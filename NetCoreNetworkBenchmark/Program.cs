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
            var predefinedBenchmark = false;

            var options = new OptionSet()
            {
                { "h|?|help", "Show help",
	                v =>  showHelp = (v != null) },
                { "b|benchmark", "Run predefined full benchmark with all tests and libraries, ignores all other settings",
	                v =>  predefinedBenchmark = (v != null) },
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
                { "x|messageload=", $"Message load sent by clients (Default: {Config.MessagePayload})\nOptions: {Utilities.EnumToString<MessagePayload>()}",
	                v => Utilities.ParseOption(v, out Config.MessagePayload) },
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

            if (predefinedBenchmark)
            {
	            RunPredefinedBenchmark();
	            return;
            }

            Console.Write(Config.PrintConfiguration());
            Run();
        }

        private static void Run()
        {
	        _networkBenchmark = INetworkBenchmark.CreateNetworkBenchmark(Config.Library);

	        try
	        {
		        Utilities.WriteStep("-> Prepare Benchmark.");
		        PrepareBenchmark();
		        Utilities.WriteStepLine(" Done");

		        Utilities.WriteStep($"-> Run Benchmark {Config.Library}...");
		        RunBenchmark();
		        Utilities.WriteStepLine(" Done");
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine($"Error when running Library {Config.Library}" +
		                          $"\n{e.Message}\n{e.StackTrace}");
	        }
	        finally
	        {
		        Utilities.WriteStep("-> Clean up.");
		        CleanupBenchmark();
		        Utilities.WriteStepLine(" Done");
				Console.Write(Config.PrintStatistics());
	        }
        }

        private static void PrepareBenchmark()
        {
	        Config.PrepareForNewBenchmark();
	        _networkBenchmark.Initialize(Config);
	        Utilities.WriteStep(".");

	        var serverTask =  _networkBenchmark.StartServer();
	        var clientTask =  _networkBenchmark.StartClients();
	        serverTask.Wait();
	        clientTask.Wait();
	        Utilities.WriteStep(".");

	        _networkBenchmark.ConnectClients().Wait();
        }

        private static void RunBenchmark()
        {
	        Config.BenchmarkData.Reset();
	        Config.BenchmarkData.StartBenchmark();
	        _networkBenchmark.StartBenchmark();
	        Thread.Sleep(Config.TestDurationInSeconds * 1000);
	        _networkBenchmark.StopBenchmark();
	        Config.BenchmarkData.StopBenchmark();
        }

        private static void CleanupBenchmark()
        {

	        _networkBenchmark.DisconnectClients().Wait();
	        _networkBenchmark.StopClients().Wait();
	        _networkBenchmark.DisposeClients().Wait();
	        Utilities.WriteStep(".");


	        _networkBenchmark.StopServer().Wait();
	        _networkBenchmark.DisposeServer().Wait();
	        Utilities.WriteStep(".");

	        GC.Collect();
        }

        private static void RunPredefinedBenchmark()
        {
	        Config = new BenchmarkConfiguration()
	        {
		        Address = "127.0.0.1",
		        TestType = TestType.PingPong,
		        MessageByteSize = 32,
		        MessagePayload = MessagePayload.Random,
		        NumClients = 1000,
		        Name = "1",
		        ParallelMessagesPerClient = 1,
		        PrintSteps = false,
		        TestDurationInSeconds = 60,
		        TickRateServer = 60,
		        TickRateClient = 60
	        };

	        Console.Write(Config.PrintConfiguration());
	        RunWithAllLibraries();

	        Config.Name = "2";
	        Config.MessageByteSize = 32;
	        Config.NumClients = 100;
	        Config.ParallelMessagesPerClient = 1000;

	        Console.Write(Config.PrintConfiguration());
	        RunWithAllLibraries();
        }

        private static void RunWithAllLibraries()
        {
	        RunWithLibrary(NetworkLibrary.ENet);
	        RunWithLibrary(NetworkLibrary.LiteNetLib);
	        RunWithLibrary(NetworkLibrary.NetCoreServer);
        }

        private static void RunWithLibrary(NetworkLibrary library)
        {
	        Config.Library = library;
	        Run();
	        Thread.Sleep(500);
	        GC.Collect();
        }
    }
}
