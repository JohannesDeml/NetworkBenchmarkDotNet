// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteNetLibBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
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
		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoServer(setup, statistics);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, setup, statistics);
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
