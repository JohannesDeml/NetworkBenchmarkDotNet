// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagePayload.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkBenchmark
{
	public enum MessagePayload
	{
		/// <summary>
		/// Random bytes that always use the seed of 0
		/// They are always the same bytes for a gives message size
		/// </summary>
		Random,

		/// <summary>
		/// Each byte is 0x00
		/// </summary>
		Zeros,

		/// <summary>
		/// Each byte is 0xff
		/// </summary>
		Ones
	}
}
