# Network Benchmark .NET

*Low Level .NET 5 Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetworkBenchmarkDotNet/all.svg)](../../releases) [![.NET 5.0](https://img.shields.io/badge/.NET-5.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/5.0)

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

* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.7)
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
* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 5.0.15)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
  * [Unity Client Example](https://github.com/JohannesDeml/Unity-Net-Core-Networking-Sockets)



## Benchmarks

### Benchmark [PingPongUnreliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/PerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **1 message** each with the server with **unreliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time.

### Benchmark [PingPongBatchedUnreliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/PerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **10 messages** each with the server with **unreliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for multiplexing / message merging performance.

### Benchmark [PingPongReliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/PerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **1 message** each with the server with **reliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time.

### Benchmark [SampleEchoSimple](./NetworkBenchmarkDotNet/PredefinedBenchmarks/SamplingBenchmark.cs)

Runs the benchmark with **1** client, which pingpong **10 messages** each with the server. The benchmark runs until a total of **100,000** messages are sent to the server and back to the clients. Message size is **128 bytes**.  
This test collects information about generated garbage and CPU times while running the benchmark. Those results can be analyzed with [PerfView](https://github.com/microsoft/perfview) on Windows.

## Benchmark Results

### Ubuntu 20.04

To reproduce the benchmarks, run `./NetworkBenchmarkDotNet -b Essential`.   
[Hardware Details](https://pcpartpicker.com/b/Wtykcf)

``` ini
BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.103
  [Host]     : .NET Core 5.0.3 (CoreCLR 5.0.321.7203, CoreFX 5.0.321.7203), X64 RyuJIT
  Job-YNISTP : .NET Core 5.0.3 (CoreCLR 5.0.321.7203, CoreFX 5.0.321.7203), X64 RyuJIT

Platform=X64  Runtime=.NET Core 5.0  Concurrent=True  
Force=True  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  Version=0.9.0  OS=Linux 5.8.0-43-generic #49~20.04.1-Ubuntu SMP Fri Feb 5 09:57:56 UTC 2021  
DateTime=02/18/2021 16:18:27  
```
|                    Method |       Library | Transmission | Clients |      Throughput |       Mean |    Error |   StdDev |
|-------------------------- |-------------- |------------- |--------:|----------------:|-----------:|---------:|---------:|
| **PingPongReliable** |       **ENet** |     **Reliable** |     **500** | **90,333 msg/s** | **5.535 s** | **0.0311 s** | **0.0205 s** |
|        PingPongUnreliable |          ENet |   Unreliable |     500 |   185,112 msg/s | 2,701.1 ms | 21.57 ms | 12.83 ms |
| PingPongBatchedUnreliable |          ENet |   Unreliable |     500 | 1,129,598 msg/s |   442.6 ms |  4.83 ms |  2.87 ms |
|        **PingPongUnreliable** | **NetCoreServer** |   **Unreliable** |     **500** |    **96,514 msg/s** | **5,180.6 ms** | **64.21 ms** | **42.47 ms** |
| PingPongBatchedUnreliable | NetCoreServer |   Unreliable |     500 |    97,245 msg/s | 5,141.6 ms | 55.11 ms | 36.45 ms |
| **PingPongReliable** | **LiteNetLib** |     **Reliable** |     **500** | **82,804 msg/s** | **6.038 s** | **0.0671 s** | **0.0444 s** |
|        PingPongUnreliable |    LiteNetLib |   Unreliable |     500 |    91,222 msg/s | 5,481.2 ms | 51.56 ms | 34.11 ms |
| PingPongBatchedUnreliable |    LiteNetLib |   Unreliable |     500 |   251,421 msg/s | 1,988.7 ms | 56.09 ms | 33.38 ms |

![Benchmark Results](./Docs/Results-Ubuntu20.04.1-.NETCore5.0.png)

### Windows 10
To reproduce the benchmarks, run `./NetworkBenchmarkDotNet -b Essential`.  
[Hardware Details](https://pcpartpicker.com/b/8MMcCJ) (Note that this machine has a lot more performance than the linux machine)

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.103
  [Host]     : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT
  Job-ODIPRB : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT

Platform=X64  Runtime=.NET Core 5.0  Concurrent=True  
Force=True  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  Version=0.9.0  OS=Microsoft Windows 10.0.19042  
DateTime=02/18/2021 16:18:02  
```
|                    Method |       Library | Transmission | Clients |    Throughput |       Mean |       Error |    StdDev |
|-------------------------- |-------------- |------------- |--------:|--------------:|-----------:|------------:|----------:|
| **PingPongReliable** |       **ENet** |     **Reliable** |     **500** | **51,718 msg/s** | **9.668 s** | **0.1468 s** | **0.0768 s** |
|        PingPongUnreliable |          ENet |   Unreliable |     500 |  93,960 msg/s | 5,321.4 ms | 1,040.97 ms | 688.54 ms |
| PingPongBatchedUnreliable |          ENet |   Unreliable |     500 | 687,775 msg/s |   727.0 ms |    84.13 ms |  55.65 ms |
|        **PingPongUnreliable** | **NetCoreServer** |   **Unreliable** |     **500** |  **72,975 msg/s** | **6,851.6 ms** |    **41.01 ms** |  **27.12 ms** |
| PingPongBatchedUnreliable | NetCoreServer |   Unreliable |     500 |  78,644 msg/s | 6,357.8 ms |    42.78 ms |  25.46 ms |
| **PingPongReliable** | **LiteNetLib** |     **Reliable** |     **500** | **88,463 msg/s** | **5.652 s** | **0.0231 s** | **0.0138 s** |
|        PingPongUnreliable |    LiteNetLib |   Unreliable |     500 |  90,985 msg/s | 5,495.4 ms |    30.93 ms |  20.46 ms |
| PingPongBatchedUnreliable |    LiteNetLib |   Unreliable |     500 | 771,852 msg/s |   647.8 ms |    13.48 ms |   8.92 ms |

![Benchmark Results](./Docs/Results-Windows10-.NETCore5.0.png)

### Notes

* The tests perform very different on Linux compared to Windows 10, since there are a lot of client threads involved and Linux seems to handle them a lot better.
* Creation, Connection and Disconnection and Disposal of the Server and Clients is not included in the performance benchmarks, but is included in the .nettrace files from the Sampling benchmark.
* Since the clients and the server run on the same machine, there is a lot less network latency as in a real world application. On the other hand, the CPU pressure is a lot higher than for a normal server, since all the clients get there own threads and run on the same machine. Take the results with a grain of salt.
* To access the Sampling results, you can use [PerfView](https://github.com/microsoft/perfview) to open the `.nettrace` files.
* Kcp2k has been recently added and might have some room for improvements. Especially using `Thread.Sleep` on Windows creates [noticeable delays](https://social.msdn.microsoft.com/Forums/vstudio/en-US/facc2b57-9a27-4049-bb32-ef093fbf4c29/threadsleep1-sleeps-for-156-ms?forum=clr). For now it is excluded of the predefined benchmarks, until its execution and cleanup are improved.



## Installation

Make sure you have [.Net 5 SDK](https://dotnet.microsoft.com/download) installed.

Then just open the solution file with Visual Studio/Rider/Visual Studio Code and build it. Note that results of the benchmarks can be very different with a different operating system and hardware.

## Usage
In general there are two different types of benchmarks: Custom and predefined benchmarks. Custom benchmarks and be defined through the [command-line options](#command-line-options). Predefined Benchmarks are set in code and are executed through [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet). They are used for the statistics presented here and are more accurate and better reproducible then Custom benchmarks. However, they also take a longer time to finish. You can also run the clients and server on different machines to test the libraries over your local network or remotely (see [remote benchmarks](#remote-benchmarks)).

### Custom Benchmarks

You can run custom benchmarks through the command-line. Use can test multiple settings and its combinations in an easy and quick way. The tests will just run once and are not as accurate as running a predefined benchmark. An example for running a benchmark might be `./NetworkBenchmarkDotNet --library ENet --transmission Unreliable --clients 100 --duration 10 `

#### Command-line Options (`./NetworkBenchmarkDotNet --help`)
```
Usage:
  NetworkBenchmarkDotNet [options]

Options:
  -b, --benchmark <All|Custom|Essential|Performance|Quick|Sampling>    Run predefined benchmarks [default: Custom]
  -m, --execution-mode <Client|Complete|Server>                        Control what parts to run [default: Complete]
  -t, --test <PingPong>                                                Test type [default: PingPong]
  --transmission <Reliable|Unreliable>                                 Transmission type [default: Unreliable]
  -l, --library <ENet|Kcp2k|LiteNetLib|NetCoreServer>                  Library target [default: ENet]
  -d, --duration <duration>                                            Test duration in seconds (-1 for manual stopping) [default: 10]
  --address <address>                                                  IP Address, can be ipv4 (e.g. 127.0.0.1) or ipv6 (e.g. ::1) [default: ::1]
  --port <port>                                                        Socket Port [default: 3330]
  --clients <clients>                                                  # Simultaneous clients [default: 500]
  --parallel-messages <parallel-messages>                              # Parallel messages per client [default: 1]
  --message-byte-size <message-byte-size>                              Message byte size sent by clients [default: 32]
  --message-payload <Ones|Random|Zeros>                                Message load sent by clients [default: Random]
  --verbose                                                            Verbose output of test steps and errors [default: True]
  --client-tick-rate <client-tick-rate>                                Client ticks per second if supported [default: 60]
  --server-tick-rate <server-tick-rate>                                Server ticks per second if supported [default: 60]
  --version                                                            Show version information
  -?, -h, --help                                                       Show help and usage information

```

### Predefined Benchmarks

Predefined benchmarks take some time to run, but generate reproducible numbers. The easiest way to run a predefined benchmark is to run `win-benchmark.bat` on windows or `sh linux-benchmark.sh` on windows.

#### Types

* **Quick** (<1min): Runs a quick benchmark with whatever is set in [QuickBenchmark.cs](../../blob/master/NetworkBenchmarkDotNet/PredefinedBenchmarks/QuickBenchmark.cs)
* **Performance** (>15min): High Performance statistical test with all included libraries
* **Sampling** (<1min): Test with all included libraries using cpu sampling and memory allocation statistics
* **Essential** (>15min): Running Performance + Garbage Benchmark

![Run Predefined Benchmark windows command-line screenshot](./Docs/run-predefined-benchmark.png)

### Remote Benchmarks

To test a library remotely, you can use the parameter `--execution-mode Server` and `--execution-mode Client` respectively. This setup requires to first start the server with the correct library (and probably an indefinite execution duration) on your target server, and then the client process. Here is an example:  
Server: `./NetworkBenchmarkDotNet --library ENet --transmission Reliable --execution-mode Server --address YOUR_ADDRESS -d -1`  
Client: `./NetworkBenchmarkDotNet --library ENet --transmission Reliable --execution-mode Client --address YOUR_ADDRESS --clients 100 -d 10`

If you change the address in `QuickBenchmark.cs`, you can also run a more sophisticated remote benchmark this way.

## Contributions

Your favorite library is missing, or you feel like the benchmarks are not testing everything relevant? Let's evolve the benchmark together! Either hit me up via [E-mail](mailto:public@deml.io) to discuss your idea, or [open an issue](../../issues), or make a pull request directly. There are a few rules in order to not make the benchmark too cluttered.

### Adding a Library

Your new proposed library ... 

* works with Unity as a Client
* works with .NET 5 for the server 
* uses UDP (additional RUDP would be nice)
* is Open Source (can still be commercial)
* is stable enough not to break in the benchmarks
* is in active development
* adds value to the benchmarks

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
