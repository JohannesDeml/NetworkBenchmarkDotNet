// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IThreadedClient.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;

namespace NetworkBenchmark
{
	public interface IThreadedClient : IClient
	{
		Thread ClientThread { get; }
	}
}
