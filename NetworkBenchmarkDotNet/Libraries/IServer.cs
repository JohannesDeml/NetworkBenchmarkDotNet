// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NetworkBenchmark
{
	public interface IServer : IDisposable
	{
		public bool IsStarted { get; }

		public void StartServer();
		public void StartBenchmark();
		public void StopBenchmark();
		public void StopServer();
	}
}
