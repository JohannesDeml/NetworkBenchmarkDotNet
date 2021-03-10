// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreServerBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
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

		public override IClient CreateNewClient(int id, ClientGroup clientGroup, Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, clientGroup, config, statistics);
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
