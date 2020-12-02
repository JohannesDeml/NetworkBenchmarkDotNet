// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestType.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreNetworkBenchmark
{
	public enum TestType
	{
		/// <summary>
		/// 1. Client sends <see cref="BenchmarkConfiguration.ParallelMessages"/> to the server (Ping)
		/// 2. Server sends each messages it receives back to the sending client (Pong)
		/// 3. Client again sends each message it receives back to the server and so on.
		/// Also known as Echo. Great for testing round trip if just one parallel message is used.
		/// </summary>
		PingPong
	}
}
