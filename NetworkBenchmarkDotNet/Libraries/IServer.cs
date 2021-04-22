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
		/// <summary>
		/// Server is started and ready to receive client connects
		/// Becomes true after StartServer is called
		/// </summary>
		public bool IsStarted { get; }

		/// <summary>
		/// Start the server and its listening activity
		/// </summary>
		public void StartServer();

		/// <summary>
		/// Start the benchmark. Make every received message count in the statistics
		/// </summary>
		public void StartBenchmark();

		/// <summary>
		/// Stop the benchmark. No more messages are count in the statistics
		/// </summary>
		public void StopBenchmark();

		/// <summary>
		/// Stop the server and its listening activity
		/// </summary>
		public void StopServer();

		#region ManualMode

		public void SendMessages(int messageCount);

		#endregion
	}
}
