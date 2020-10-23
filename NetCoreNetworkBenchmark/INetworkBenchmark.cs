using System;
using System.Threading.Tasks;
using NetCoreNetworkBenchmark.Enet;
using NetCoreNetworkBenchmark.LiteNetLib;
using NetCoreNetworkBenchmark.NetCoreServer;

namespace NetCoreNetworkBenchmark
{
	internal interface INetworkBenchmark
	{
		public static INetworkBenchmark CreateNetworkBenchmark(NetworkLibrary library)
		{
			switch (library)
			{
				case NetworkLibrary.ENet:
					return new ENetBenchmark();

				case NetworkLibrary.NetCoreServer:
					return new NetCoreServerBenchmark();

				case NetworkLibrary.LiteNetLib:
					return new LiteNetLibBenchmark();

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
		/// <returns>Awaitable Task for when server finished starting.</returns>
		Task StartServer();

		/// <summary>
		/// Start the clients with the configuration defined during initialization.
		/// Note: Server might not be started at that time
		/// </summary>
		/// <returns>Awaitable Task for when clients are created and finished starting.</returns>
		Task StartClients();

		/// <summary>
		/// Connect the running clients with the server.
		/// </summary>
		/// <returns>Awaitable Task for when all clients connected with the server.</returns>
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
		/// <returns>Awaitable Task for when all clients are disconnected from the server.</returns>
		Task DisconnectClients();

		/// <summary>
		/// Stop server
		/// </summary>
		/// <returns>Awaitable Task for when server is stopped.</returns>
		Task StopServer();

		/// <summary>
		/// Stop all clients
		/// </summary>
		/// <returns>Awaitable Task for when all clients are stopped.</returns>
		Task StopClients();

		/// <summary>
		/// Cleanup clients
		/// </summary>
		/// <returns>Awaitable Task for when disposing is finished.</returns>
		Task DisposeClients();

		/// <summary>
		/// Cleanup server
		/// </summary>
		/// <returns>Awaitable Task for when disposing is finished.</returns>
		Task DisposeServer();
	}
}
