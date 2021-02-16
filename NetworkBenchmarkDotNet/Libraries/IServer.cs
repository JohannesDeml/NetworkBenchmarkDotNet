// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public interface IServer
	{
		public bool IsStarted { get; }
	}
}
