// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutionMode.cs">
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
	/// <summary>
	/// Adds a possibility to run the clients and server separately to test within the local network or with a remote server
	/// The clients are responsible for generating the statistics.
	/// There is no option for controlling which library should be used for the server, so the correct server needs to be started beforehand
	/// </summary>
	[Flags]
	public enum ExecutionMode
	{
		/// <summary>
		/// Run the clients
		/// </summary>
		Client = 1 << 0,

		/// <summary>
		/// Run the server
		/// </summary>
		Server = 1 << 1,

		/// <summary>
		/// Run both clients and server
		/// </summary>
		Complete = (1 << 2) - 1
	}
}
