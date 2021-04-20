// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ENetBenchmark.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using ENet;

namespace NetworkBenchmark.Enet
{
	internal class ENetBenchmark : ANetworkBenchmark
	{
		public override void Initialize(Configuration config, BenchmarkStatistics benchmarkStatistics)
		{
			ENet.Library.Initialize();
			base.Initialize(config, benchmarkStatistics);
		}

		protected override IServer CreateNewServer(Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoServer(config, statistics);
		}

		protected override IClient CreateNewClient(int id, Configuration config, BenchmarkStatistics statistics)
		{
			return new EchoClient(id, config, statistics);
		}

		public override void Deinitialize()
		{
			base.Deinitialize();
			ENet.Library.Deinitialize();
		}

		public static PacketFlags GetPacketFlags(TransmissionType transmissionType)
		{
			switch (transmissionType)
			{
				case TransmissionType.Reliable:
					return PacketFlags.Reliable | PacketFlags.Unsequenced;
				case TransmissionType.Unreliable:
					return PacketFlags.None | PacketFlags.Unsequenced | PacketFlags.Unthrottled;
				default:
					throw new ArgumentOutOfRangeException(nameof(transmissionType), $"Transmission Type {transmissionType} not supported");
			}
		}
	}
}
