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
		protected override IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoServer(config, statistics);
		}

		public override IClient CreateNewClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, clientGroup, config, statistics);
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
