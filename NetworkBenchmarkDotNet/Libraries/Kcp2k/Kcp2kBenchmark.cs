// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kcp2kBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using kcp2k;

namespace NetworkBenchmark.Kcp2k
{
	internal class Kcp2kBenchmark : ANetworkBenchmark
	{
		protected override IServer CreateNewServer(BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoServer(setup, statistics);
		}

		protected override IClient CreateNewClient(int id, BenchmarkSetup setup, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, setup, statistics);
		}

		public static KcpChannel GetChannel(TransmissionType transmissionType)
		{
			switch (transmissionType)
			{
				case TransmissionType.Reliable:
					return KcpChannel.Reliable;
				case TransmissionType.Unreliable:
					return KcpChannel.Reliable;
				default:
					throw new ArgumentOutOfRangeException(nameof(transmissionType), $"Transmission Type {transmissionType} not supported");
			}
		}
	}
}
