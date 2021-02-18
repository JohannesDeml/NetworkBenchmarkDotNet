// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceBenchmark.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace NetworkBenchmark
{
	[Config(typeof(PerformanceBenchmarkConfig))]
	public class PerformanceBenchmark : APredefinedBenchmark
	{
		/// <summary>
		/// These libraries will be used in the benchmarks with unreliable transmission
		/// </summary>
		public IEnumerable<object> UnreliableTransmissionLibraries()
		{
			yield return NetworkLibrary.ENet;
			yield return NetworkLibrary.LiteNetLib;
			yield return NetworkLibrary.NetCoreServer;
		}

		/// <summary>
		/// These libraries will be used in the benchmarks with reliable transmission
		/// </summary>
		public IEnumerable<object> ReliableTransmissionLibraries()
		{
			yield return NetworkLibrary.ENet;
			yield return NetworkLibrary.LiteNetLib;
		}

		public NetworkLibrary Library { get; set; }
		protected override BenchmarkMode Mode => BenchmarkMode.Performance;
		public override int ClientCount { get; set; } = 500;
		public override int MessageTarget { get; set; } = 500 * 1000;

		protected override NetworkLibrary LibraryTarget => Library;

		[GlobalSetup(Target = nameof(PingPongUnreliable))]
		public void PreparePingPongUnreliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(PingPongBatchedUnreliable))]
		public void PreparePingPongBatchedUnreliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 10;
			config.MessageByteSize = 32;
			PrepareBenchmark();
		}

		[GlobalSetup(Target = nameof(PingPongReliable))]
		public void PreparePingPongReliable()
		{
			BenchmarkCoordinator.ApplyPredefinedConfiguration();
			var config = BenchmarkCoordinator.Config;

			config.ParallelMessages = 1;
			config.MessageByteSize = 32;
			config.Transmission = TransmissionType.Reliable;
			PrepareBenchmark();
		}


		[Benchmark]
		[ArgumentsSource(nameof(UnreliableTransmissionLibraries))]
		public long PingPongUnreliable(NetworkLibrary library)
		{
			Library = library;
			return RunBenchmark();
		}

		[Benchmark]
		[ArgumentsSource(nameof(UnreliableTransmissionLibraries))]
		public long PingPongBatchedUnreliable(NetworkLibrary library)
		{
			Library = library;
			return RunBenchmark();
		}

		[Benchmark]
		[ArgumentsSource(nameof(ReliableTransmissionLibraries))]
		public long PingPongReliable(NetworkLibrary library)
		{
			Library = library;
			return RunBenchmark();
		}

		public override string ToString()
		{
			return "PerformanceBenchmark";
		}
	}
}
