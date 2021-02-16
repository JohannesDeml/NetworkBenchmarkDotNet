// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClient.cs">
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
	public interface IClient : IDisposable
	{
		public bool IsConnected { get; }
		public bool IsStopped { get; }
		public bool IsDisposed { get; }

		public void StartClient();
		public void StartBenchmark();
		public void StopBenchmark();
		public void StopClient();
		public void DisconnectClient();

	}
}
