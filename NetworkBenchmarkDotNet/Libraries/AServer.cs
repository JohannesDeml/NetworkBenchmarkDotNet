// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public abstract class AServer : IServer
	{
		public abstract bool IsStarted { get; }

		protected volatile bool benchmarkPreparing;
		protected volatile bool listen;
		protected volatile bool benchmarkRunning;


		public virtual void StartServer()
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

		public virtual void StopServer()
		{
			listen = false;
		}

		public abstract void Dispose();
	}
}
