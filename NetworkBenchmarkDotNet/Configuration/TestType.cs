// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestType.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public enum TestType
	{
		/// <summary>
		/// 1. Client sends <see cref="Configuration.ParallelMessages"/> to the server (Ping)
		/// 2. Server sends each messages it receives back to the sending client (Pong)
		/// 3. Client again sends each message it receives back to the server and so on.
		/// Also known as Echo. Great for testing round trip if just one parallel message is used.
		/// </summary>
		PingPong,

		/// <summary>
		/// Messages are sent manually by clients or server
		/// Helps to understand data by sniffing the traffic or to debug libraries
		/// Can also be used in conjunction with a ping-pong server process to get direct responses from the server
		/// <example>
		/// <code>
		/// c 1 // sends one message for each client
		/// s 5 // send five messages to each client the server is connected to
		/// q   // quit
		/// </code>
		/// </example>
		/// </summary>
		Manual
	}
}
