// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AClient.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public abstract class AClient : IClient
	{
		public abstract bool IsConnected { get; }

		public abstract bool IsDisposed { get; }

		/// <summary>
		/// Benchmark is preparing to be run
		/// </summary>
		protected volatile bool BenchmarkPreparing;
		/// <summary>
		/// Client should listen for incoming messages
		/// </summary>
		protected volatile bool Listen;
		/// <summary>
		/// Is a benchmark running (and therefore messages should be counted in the statistics)
		/// </summary>
		protected volatile bool BenchmarkRunning;

		protected int id;
		protected ClientGroup clientGroup;

		protected AClient(int id, ClientGroup clientGroup)
		{
			this.id = id;
			this.clientGroup = clientGroup;
		}

		public virtual void StartClient()
		{
			Listen = true;
			BenchmarkPreparing = true;
		}

		public abstract void ConnectClient();

		public abstract void Tick(int elapsedMs);

		public virtual void StartBenchmark()
		{
			BenchmarkPreparing = false;
			BenchmarkRunning = true;
		}

		public virtual void StopBenchmark()
		{
			BenchmarkRunning = false;
		}

		public virtual void StopClient()
		{
			Listen = false;
		}

		public abstract void DisconnectClient();

		public virtual void Dispose()
		{
			clientGroup.OnClientDisposed();
		}

		protected virtual void OnConnected()
		{
			clientGroup.OnClientConnected();
		}

		protected virtual void OnDisconnected()
		{
			if (BenchmarkPreparing || BenchmarkRunning)
			{
				Utilities.WriteVerboseLine($"Client {id} disconnected while benchmark is running.");
			}

			clientGroup.OnClientDisconnected();
		}
	}
}
