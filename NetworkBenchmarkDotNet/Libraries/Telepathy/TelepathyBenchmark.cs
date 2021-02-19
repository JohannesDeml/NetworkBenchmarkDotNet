// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelepathyBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace NetworkBenchmark.Telepathy
{
	internal class TelepathyBenchmark : ANetworkBenchmark
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
					return;
				case TransmissionType.Unreliable:
					Console.WriteLine("Telepathy does not support unreliable message delivery");
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(transmissionType), $"Transmission Type {transmissionType} not supported");
			}
		}
	}
}
