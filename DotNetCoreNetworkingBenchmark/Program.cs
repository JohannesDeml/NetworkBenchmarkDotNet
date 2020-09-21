using System;
using NDesk.Options;

// ENet 2.4.3 (https://github.com/nxrighthere/ENet-CSharp)
using ENet;
// LiteNetLib 0.9.3.2 (https://github.com/RevenantX/LiteNetLib)
using LiteNetLib;

namespace DotNetCoreNetworkingBenchmark
{
    internal enum NetworkLibrary
    {
        ENet,
        LiteNetLib
    }
    
    class Program
    {
        public static BenchmarkConfiguration Config;
        
        static void Main(string[] args)
        {
            Config = new BenchmarkConfiguration();

            var options = new OptionSet()
            {
                { "h|?|help", "Show help",   v => Config.ShowHelp = v != null },
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

            if (Config.ShowHelp)
            {
                Console.WriteLine("Usage:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            Console.WriteLine(Config.PrintConfiguration());
        }
    }
}