# Network Benchmark .NET

*Low Level .NET Core / .NET 5 Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetworkBenchmarkDotNet/all.svg)](../../releases) [![.NET 5.0](https://img.shields.io/badge/.NET-5.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/5.0) [![.NET Core 3.1](https://img.shields.io/badge/.NET_Core-3.1-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Table of Contents

1. [Description](#description)
2. [Benchmarks](#benchmarks)
3. [Benchmark Results](#benchmark-results)
4. [Installation](#installation)
5. [Usage](#usage)
6. [Contributions](#contributions)



## Description

NBN is a benchmark for low level networking libraries using UDP and can be used with [Unity](https://unity3d.com) and for [.Net Core](https://en.wikipedia.org/wiki/.NET_Core) standalone server applications. The benchmark focuses on latency, performance and scalability.

### Supported Libraries

* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.6)
  * Wrapper for [ENet](https://github.com/lsalzman/enet), building a reliable sequenced protocol on top of UDP
  * Max concurrent connections are limited to 4095 due to the protocol
  * Packetsize overhead: 10 bytes
  * [Unity Client Example](https://github.com/JohannesDeml/ENetUnityMobile)
* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (v 0.9.4)
  * Very feature-rich library
  * Packetsize overhead: 1 byte for unreliable, 4 bytes for reliable
  * [Unity Client Example](https://github.com/RevenantX/NetGameExample)
* [Kcp2k](https://github.com/vis2k/kcp2k) (v 1.8.0)
  * Port of KCP with 100% C# Code, Future Technology for [Mirror-NG](https://github.com/MirrorNG/MirrorNG)
  * Packetsize overhead: 24 byte
  * [Unity Example](https://github.com/vis2k/kcp2k)
* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 3.0.22)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
  * [Unity Client Example](https://github.com/JohannesDeml/Unity-Net-Core-Networking-Sockets)



## Benchmarks

### Benchmark [Performance1](./NetworkBenchmarkDotNet/PredefinedBenchmarks/PerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **1 message** each with the server. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time.

### Benchmark [Performance2](./NetworkBenchmarkDotNet/PredefinedBenchmarks/PerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **10 messages** each with the server. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for multiplexing / message merging performance.

### Benchmark [Garbage](./NetworkBenchmarkDotNet/PredefinedBenchmarks/GarbageBenchmark.cs)

Runs the benchmark with **10** clients, which pingpong **10 messages** each with the server. The benchmark runs until a total of **10,000** messages are sent to the server and back to the clients. Message size is **128 bytes**.  
This test collects information about generated garbage while running the benchmark.

## Benchmark Results

### Ubuntu 20.04

To reproduce the benchmarks, run `./NetworkBenchmarkDotNet -b Essential`.   
[Detailed Benchmark Hardware](https://pcpartpicker.com/b/Wtykcf)


``` ini
BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.103
  [Host]     : .NET Core 5.0.3 (CoreCLR 5.0.321.7203, CoreFX 5.0.321.7203), X64 RyuJIT
  Job-IJBNLS : .NET Core 5.0.3 (CoreCLR 5.0.321.7203, CoreFX 5.0.321.7203), X64 RyuJIT

Platform=X64  Runtime=.NET Core 5.0  Concurrent=True  
Force=True  Server=True  InvocationCount=1  
IterationCount=20  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  Version=0.8.1  OS=Linux 5.8.0-41-generic #46~20.04.1-Ubuntu SMP Mon Jan 18 17:52:23 UTC 2021  
DateTime=02/16/2021 11:49:25  
```
|       Method |       Library | Clients |      Throughput |       Mean |       Error |      StdDev |
|------------- |-------------- |--------:|----------------:|-----------:|------------:|------------:|
| **Performance1** |          **ENet** |     **500** |   **185,238 msg/s** | **2,699.2 ms** |    **15.25 ms** |    **15.66 ms** |
| Performance2 |          ENet |     500 | 1,123,648 msg/s |   445.0 ms |     2.71 ms |     2.90 ms |
| **Performance1** | **NetCoreServer** |     **500** |   **102,402 msg/s** | **4,882.7 ms** |    **18.56 ms** |    **21.38 ms** |
| Performance2 | NetCoreServer |     500 |   107,236 msg/s | 4,662.6 ms |    22.80 ms |    26.26 ms |
| **Performance1** |    **LiteNetLib** |     **500** |    **81,932 msg/s** | **6,102.6 ms** |    **43.66 ms** |    **50.27 ms** |
| Performance2 |    LiteNetLib |     500 |   307,560 msg/s | 1,625.7 ms |     9.74 ms |    10.83 ms |
| **Performance1** |         **Kcp2k** |     **500** |    **65,381 msg/s** | **7,647.5 ms** |   **873.18 ms** |   **970.53 ms** |
| Performance2 |         Kcp2k |     500 |    51,884 msg/s | 9,636.8 ms | 1,815.34 ms | 2,090.55 ms |

![Benchmark Results](./Docs/PerformanceLinux.png)

### Windows 10
To reproduce the benchmarks, run `./NetworkBenchmarkDotNet -b Essential`.  
[Detailed Benchmark Hardware](https://pcpartpicker.com/b/8MMcCJ) (Note that this machine has a lot more performance than the linux machine)


``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  Job-DLLJEE : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT

Platform=X64  Runtime=.NET Core 5.0  Concurrent=True  
Force=True  Server=True  InvocationCount=1  
IterationCount=20  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  Version=0.8.1  OS=Microsoft Windows 10.0.19042  
DateTime=02/16/2021 11:49:06  
```
|       Method |       Library | Clients |    Throughput |        Mean |     Error |    StdDev |
|------------- |-------------- |--------:|--------------:|------------:|----------:|----------:|
| **Performance1** |          **ENet** |     **500** |  **96,028 msg/s** |  **5,206.8 ms** | **272.20 ms** | **313.46 ms** |
| Performance2 |          ENet |     500 | 696,194 msg/s |    718.2 ms |  42.12 ms |  48.51 ms |
| **Performance1** | **NetCoreServer** |     **500** |  **71,063 msg/s** |  **7,036.0 ms** |  **37.55 ms** |  **43.24 ms** |
| Performance2 | NetCoreServer |     500 |  69,418 msg/s |  7,202.7 ms |  18.91 ms |  21.02 ms |
| **Performance1** |    **LiteNetLib** |     **500** |  **93,363 msg/s** |  **5,355.5 ms** |  **30.85 ms** |  **34.29 ms** |
| Performance2 |    LiteNetLib |     500 | 848,954 msg/s |    589.0 ms |   7.45 ms |   8.28 ms |
| **Performance1** |         **Kcp2k** |     **500** |  **37,782 msg/s** | **13,233.7 ms** | **420.19 ms** | **483.89 ms** |
| Performance2 |         Kcp2k |     500 | 120,659 msg/s |  4,143.9 ms |  34.13 ms |  39.30 ms |


![Benchmark Results](./Docs/PerformanceWindows.png)

### Notes

* The tests perform very different on Linux compared to Windows 10, since there are a lot of client threads involved and Linux seems to handle them a lot better.
* Creation, Connection and Disconnection and Disposal of the Server and Clients is not included in the performance benchmarks, but is included in the Garbage benchmark.
* Since the clients and the server run on the same machine, there is a lot less network latency as in a real world application. On the other hand, the CPU pressure is a lot higher than for a normal server, since all the clients get there own threads and run on the same machine. Take the results with a grain of salt.
* To access the Garbage results, you can use [PerfView](https://github.com/microsoft/perfview) to open the `.nettrace` files.
* KCP has been recently added and might have some room for improvements. Especially using `Thread.Sleep` on Windows creates [noticeable delays](https://social.msdn.microsoft.com/Forums/vstudio/en-US/facc2b57-9a27-4049-bb32-ef093fbf4c29/threadsleep1-sleeps-for-156-ms?forum=clr).



## Installation

Make sure you have [.NetCore SDK](https://dotnet.microsoft.com/download) 3.1 & 5.0 installed.

Then just open the solution file with Visual Studio/Rider/Visual Studio Code and build it. Note that results of the benchmarks can be very different with a different operating system and hardware.

## Usage
In general there are two different types of benchmarks: Custom and predefined benchmarks. Custom benchmarks and be defined through the [command-line options](#command-line-options). Predefined Benchmarks are set in code and are executed through [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet). They are used for the statistics presented here and are more accurate and better reproducible then Custom benchmarks. However, they also take a longer time to finish.

### Custom Benchmarks

You can run custom benchmarks through the command-line. Use can test multiple settings and its combinations in an easy and quick way. The tests will just run once and are not as accurate as running a predefined benchmark. An example for running a benchmark might be `./NetworkBenchmarkDotNet --library ENet --transmission Unreliable --clients 100 --duration 10 `

#### Command-line Options (`./NetworkBenchmarkDotNet --help`)
```
Usage:
  NetworkBenchmarkDotNet [options]

Options:
  -b, --benchmark <All|Custom|Essential|Garbage|Performance|Quick>    Run predefined benchmarks [default: Custom]
  -t, --test <PingPong>                                               Test type [default: PingPong]
  --transmission <Reliable|Unreliable>                                Transmission type [default: Unreliable]
  -l, --library <ENet|Kcp2k|LiteNetLib|NetCoreServer>                 Library target [default: ENet]
  -d, --duration <duration>                                           Test duration in seconds [default: 10]
  --address <address>                                                 IP Address, can be ipv4 (e.g. 127.0.0.1) or ipv6 (e.g. ::1) [default: ::1]
  --port <port>                                                       Socket Port [default: 3330]
  --clients <clients>                                                 # Simultaneous clients [default: 500]
  --parallel-messages <parallel-messages>                             # Parallel messages per client [default: 1]
  --message-byte-size <message-byte-size>                             Message byte size sent by clients [default: 32]
  --message-payload <Ones|Random|Zeros>                               Message load sent by clients [default: Random]
  --verbose                                                           Verbose output of test steps and errors [default: True]
  --client-tick-rate <client-tick-rate>                               Client ticks per second if supported [default: 60]
  --server-tick-rate <server-tick-rate>                               Server ticks per second if supported [default: 60]
  --version                                                           Show version information
  -?, -h, --help                                                      Show help and usage information
```

### Predefined Benchmarks

Predefined benchmarks take some time to run, but generate reproducible numbers. The easiest way to run a predefined benchmark is to run `win-benchmark.bat` on windows or `sh linux-benchmark.sh` on windows.

#### Types

* **Quick** (<1min): Runs a quick benchmark with whatever is set in [QuickBenchmark.cs](../../blob/master/NetworkBenchmarkDotNet/PredefinedBenchmarks/QuickBenchmark.cs)
* **Performance** (>15min): High Performance statistical test with all included libraries
* **Garbage** (<1min): Test with all included libraries using cpu sampling and memory allocation statistics
* **Essential** (>15min): Running Performance + Garbage Benchmark

![Run Predefined Benchmark windows command-line screenshot](./Docs/run-predefined-benchmark.png)

## Contributions

Your favorite library is missing, or you feel like the benchmarks are not testing everything relevant? Let's evolve the benchmark together! Either hit me up via [E-mail](mailto:public@deml.io) to discuss your idea, or [open an issue](../../issues), or make a pull request directly. There are a few rules in order to not make the benchmark too cluttered.

### Adding a Library

Your new proposed library ... 

* works with Unity as a Client
* works with .Net Core (or .NET 5) for the server 
* uses UDP (additional RUDP would be nice)
* is Open Source (can still be commercial)
* is stable enough not to break in the benchmarks
* is in active development
* is interesting/relevant for others

#### How to add a library

1. Add a new folder inside the NetworkBenchmarkDotNet solution with the name of your library
2. Add a script called `YourLibraryBenchmark.cs` which implements [ANetworkBenchmark](../../blob/master/NetworkBenchmarkDotNet/Libraries/ANetworkBenchmark.cs)
3. Add your library name to the [NetworkLibrary](../../blob/master/NetworkBenchmarkDotNet/Libraries/NetworkLibrary.cs) enum
4. Add your Implementation Constructor to `INetworkBenchmark.CreateNetworkBenchmark()`
5. Use the `-l ` argument (or `BenchmarkSetup.Library`) to test your library and if everything works as expected.
6. Change `[Params(NetworkLibrary.Kcp2k)]` in `QuickBenchmark.cs` to your library and run `./NetworkBenchmarkDotNet -b Quick` to see, if your library works with high CCU and looping benchmarks with BenchmarkDotNet
8. Create a PR including your benchmark results 🎉



## License

[MIT](./LICENSE)
