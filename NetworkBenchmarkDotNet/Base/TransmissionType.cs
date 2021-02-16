// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransmissionType.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public enum TransmissionType
	{
		/// <summary>
		/// Library makes sure the packet will receive its destination.
		/// Not supported by NetCoreServer
		/// </summary>
		Reliable,
		
		/// <summary>
		/// Normal UDP behavior, packet is sent once and might get to the receiver.
		/// Note, that for localhost, the number of lost packages is normally near zero.
		/// </summary>
		Unreliable
	}
}
