// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientGroup.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NetworkBenchmark
{
	public class ClientGroup
	{
		public bool IsConnected { get; private set; }

		public bool IsStopped => tickThread == null || !tickThread.IsAlive;

		public bool IsDisposed { get; private set; }

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

		private readonly int id;
		private readonly List<IClient> clients;
		private readonly Thread tickThread;
		private readonly Configuration config;
		private BenchmarkStatistics benchmarkStatistics;

		private int connectedClients;
		private int disposedClients;

		public ClientGroup(int id, Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			this.id = id;
			this.config = config;
			this.benchmarkStatistics = benchmarkStatistics;

			clients = new List<IClient>();
			tickThread = new Thread(TickLoop);
			tickThread.Name = $"{config.Library}ClientGroup {id}";
		}

		public void InitializeClients(ref int clientId, int clientCount, INetworkBenchmark parent)
		{
			for (int i = 0; i < clientCount; i++)
			{
				clients.Add(parent.CreateNewClient(clientId, this, config, benchmarkStatistics));
				clientId++;
			}
		}

		public void StartClients()
		{
			Listen = true;
			BenchmarkPreparing = true;

			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.StartClient();
			}

			tickThread.Start();
		}

		private void TickLoop()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.ConnectClient();
			}

			var loopDuration = Utilities.CalculateTimeout(config.ClientTickRate);
			var sw = new Stopwatch();

			while (Listen)
			{
				var lastElapsed = (int)sw.ElapsedMilliseconds;
				sw.Restart();
				if (BenchmarkRunning || BenchmarkPreparing)
				{
					Tick(lastElapsed);
				}
				else
				{
					SaveTick(lastElapsed);
				}

				var remainingLoopTime = loopDuration - (int)sw.ElapsedMilliseconds;
				if (remainingLoopTime > 0)
				{
					TimeUtilities.HighPrecisionThreadSleep(remainingLoopTime);
				}
			}
		}

		private void Tick(int elapsedMs)
		{
			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.Tick(elapsedMs);
			}
		}

		private void SaveTick(int elapsedMs)
		{
			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				if (client.IsDisposed || !client.IsConnected)
				{
					continue;
				}

				try
				{
					client.Tick(elapsedMs);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			}
		}

		public virtual void StartBenchmark()
		{
			BenchmarkPreparing = false;
			BenchmarkRunning = true;

			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.StartBenchmark();
			}
		}

		public virtual void StopBenchmark()
		{
			BenchmarkRunning = false;

			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.StopBenchmark();
			}
		}

		public virtual void StopClients()
		{
			Listen = false;

			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.StopClient();
			}
		}

		public void DisconnectClients()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.DisconnectClient();
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < clients.Count; i++)
			{
				var client = clients[i];
				client.Dispose();
			}
		}

		public void OnClientConnected()
		{
			connectedClients++;

			Debug.Assert(connectedClients >= 0 && connectedClients <= clients.Count);
			if (connectedClients == clients.Count)
			{
				IsConnected = true;
			}
		}

		public void OnClientDisconnected()
		{
			connectedClients--;

			Debug.Assert(connectedClients >= 0 && connectedClients <= clients.Count);
			if (connectedClients == 0)
			{
				IsConnected = false;
			}
		}

		public void OnClientDisposed()
		{
			disposedClients++;

			Debug.Assert(disposedClients >= 0 && disposedClients <= clients.Count);
			if (disposedClients == clients.Count)
			{
				IsDisposed = true;
			}
		}
	}
}
