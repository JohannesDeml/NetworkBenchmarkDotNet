// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreServerBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NetworkBenchmark.NetCoreServer
{
	internal class NetCoreServerBenchmark : ANetworkBenchmark
	{
		protected override IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoServer(config, statistics);
		}

		protected override IClient CreateNewClient(int id, Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, config, statistics);
		}

		public static void ProcessTransmissionType(TransmissionType transmissionType)
		{
			switch (transmissionType)
			{
				case TransmissionType.Reliable:
					Console.WriteLine("NetCoreServer with UDP does not support reliable message delivery");
					return;
				case TransmissionType.Unreliable:
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(transmissionType), $"Transmission Type {transmissionType} not supported");
			}
		}
	}
}
