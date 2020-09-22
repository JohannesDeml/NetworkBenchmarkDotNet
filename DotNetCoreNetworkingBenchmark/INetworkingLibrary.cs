using System;
using System.Threading.Tasks;
using DotNetCoreNetworkingBenchmark.NetCoreServer;

namespace DotNetCoreNetworkingBenchmark
{
	internal interface INetworkingLibrary
	{
		public static INetworkingLibrary CreateNetworkingLibrary(NetworkLibrary library)
		{
			switch (library)
			{
				case NetworkLibrary.ENet:
					throw new System.NotImplementedException();
				case NetworkLibrary.NetCoreServer:
					return new NetCoreServerLibrary();
				case NetworkLibrary.LiteNetLib:
					throw new System.NotImplementedException();
				default:
					throw new ArgumentOutOfRangeException(nameof(library), library, null);
			}
		}

		/// <summary>
		/// Initialize the network library with a defined configuration
		/// </summary>
		/// <param name="config">Configuration to use for the upcoming benchmark</param>
		void Initialize(BenchmarkConfiguration config);

		/// <summary>
		/// Start the server with the configuration defined during initialization.
		/// </summary>
		Task StartServer();

		/// <summary>
		/// Start the clients with the configuration defined during initialization.
		/// Note: Server might not be started at that time
		/// </summary>
		Task StartClients();

		/// <summary>
		/// Connect the running clients with the server.
		/// </summary>
		Task ConnectClients();

		/// <summary>
		/// Start the benchmark. No data message packs should been sent between client and server before.
		/// </summary>
		void StartBenchmark();

		/// <summary>
		/// Stop client and server communication. Message Statistics are not allowed to change after stopping the benchmark
		/// </summary>
		void StopBenchmark();

		/// <summary>
		/// Disconnect the running clients from the server.
		/// </summary>
		Task DisconnectClients();

		/// <summary>
		/// Stop server
		/// </summary>
		Task StopServer();

		/// <summary>
		/// Stop all clients
		/// </summary>
		Task StopClients();
	}
}
