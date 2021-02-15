// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkLibrary.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public enum NetworkLibrary
	{
		/// <summary>
		/// https://github.com/nxrighthere/ENet-CSharp
		/// </summary>
		ENet,

		/// <summary>
		/// https://github.com/chronoxor/NetCoreServer
		/// </summary>
		NetCoreServer,

		/// <summary>
		/// https://github.com/RevenantX/LiteNetLib
		/// </summary>
		LiteNetLib,

		/// <summary>
		/// https://github.com/vis2k/kcp2k
		/// https://github.com/JohannesDeml/kcp2k/tree/networkbenchmarkdotnet/kcp2k/Assets/kcp2k
		/// </summary>
		Kcp2k
	}
}
