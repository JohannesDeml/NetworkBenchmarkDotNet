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
		/// <summary>
		/// The client is connected to the server
		/// Becomes true after StartClient is called
		/// </summary>
		public bool IsConnected { get; }

		/// <summary>
		/// The client is disconnected and has no actively listening component
		/// (.e.g. no running listen thread)
		/// Becomes true after StopClient is called
		/// </summary>
		public bool IsStopped { get; }

		/// <summary>
		/// The client is disposed
		/// Becomes true after Dispose is called
		/// </summary>
		public bool IsDisposed { get; }

		/// <summary>
		/// Start the client and  connect it to the server
		/// If the client runs in a separate thread, start this thread here
		/// </summary>
		public void StartClient();

		/// <summary>
		/// Triggers the client to start the benchmark and send messages to the server
		/// </summary>
		public void StartBenchmark();

		/// <summary>
		/// Stops the benchmark to make sure no more received messages are counted for the benchmark
		/// </summary>
		public void StopBenchmark();

		/// <summary>
		/// Disconnect the client from the server
		/// </summary>
		public void DisconnectClient();

		/// <summary>
		/// Stop the client and its listening activity
		/// </summary>
		public void StopClient();

		#region ManualMode

		public void SendMessages(int messageCount);

		#endregion
	}
}
