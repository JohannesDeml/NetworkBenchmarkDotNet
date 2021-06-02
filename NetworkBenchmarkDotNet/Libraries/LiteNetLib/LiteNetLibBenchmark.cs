// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteNetLibBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LiteNetLib;

namespace NetworkBenchmark.LiteNetLib
{
	internal class LiteNetLibBenchmark : ANetworkBenchmark
	{
		protected override IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoServer(config, statistics);
		}

		protected override IClient CreateNewClient(int id, Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, config, statistics);
		}

		public static DeliveryMethod GetDeliveryMethod(TransmissionType transmissionType)
		{
			switch (transmissionType)
			{
				case TransmissionType.Reliable:
					return DeliveryMethod.ReliableUnordered;
				case TransmissionType.Unreliable:
					return DeliveryMethod.Unreliable;
				default:
					throw new ArgumentOutOfRangeException(nameof(transmissionType), $"Transmission Type {transmissionType} not supported");
			}
		}
	}
}
