// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INetworkBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NetworkBenchmark.Enet;
using NetworkBenchmark.Kcp2k;
using NetworkBenchmark.LiteNetLib;
using NetworkBenchmark.NetCoreServer;

namespace NetworkBenchmark
{
	public interface INetworkBenchmark
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

				case NetworkLibrary.Kcp2k:
					return new Kcp2kBenchmark();

				default:
					throw new ArgumentOutOfRangeException(nameof(library), library, null);
			}
		}

		/// <summary>
		/// Initialize the network library with a defined configuration
		/// </summary>
		/// <param name="config">Configuration to use for the upcoming benchmark</param>
		/// <param name="benchmarkData">Data object to store statistics</param>
		void Initialize(BenchmarkSetup config, BenchmarkData benchmarkData);

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

		/// <summary>
		/// De-Initialize the network library
		/// </summary>
		void Deinitialize();
	}
}
