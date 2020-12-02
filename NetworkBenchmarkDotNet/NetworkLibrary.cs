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
		LiteNetLib
	}
}
