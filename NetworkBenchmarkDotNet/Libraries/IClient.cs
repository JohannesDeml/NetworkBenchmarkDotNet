// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClient.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public interface IClient
	{
		public bool IsConnected { get; }
		public bool IsDisposed { get; }
	}
}
