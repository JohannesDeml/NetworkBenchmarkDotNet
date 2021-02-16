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

		public virtual bool IsStopped
		{
			get { return !IsConnected; }
		}

		public abstract bool IsDisposed { get; }

		protected volatile bool benchmarkPreparing;
		protected volatile bool listen;
		protected volatile bool benchmarkRunning;

		public virtual void StartClient()
		{
			listen = true;
			benchmarkPreparing = true;
		}

		public virtual void StartBenchmark()
		{
			benchmarkPreparing = false;
			benchmarkRunning = true;
		}

		public virtual void StopBenchmark()
		{
			benchmarkRunning = false;
		}

		public virtual void StopClient()
		{
			listen = false;
		}

		public abstract void DisconnectClient();

		public abstract void Dispose();
	}
}
