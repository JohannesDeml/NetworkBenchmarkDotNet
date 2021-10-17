# Network Benchmark .NET

*Low Level .NET 5 Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetworkBenchmarkDotNet/all.svg)](../../releases) [![.NET 5.0](https://img.shields.io/badge/.NET-5.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/5.0)

## Table of Contents

1. [Description](#description)
2. [Benchmark Setup](#benchmark-setup)
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
* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (v 0.9.5.2)
  * Very feature-rich library
  * Packetsize overhead: 1 byte for unreliable, 4 bytes for reliable
  * [Unity Client Example](https://github.com/RevenantX/NetGameExample)
* [Kcp2k](https://github.com/vis2k/kcp2k) (v 1.12.0)
  * Port of KCP with 100% C# Code, Future Technology for [Mirror-NG](https://github.com/MirrorNG/MirrorNG)
  * Packetsize overhead: 24 byte
  * [Unity Example](https://github.com/vis2k/kcp2k)
* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 5.1.0)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
  * [Unity Client Example](https://github.com/JohannesDeml/Unity-Net-Core-Networking-Sockets)


## Benchmark Setup

### Hardware

* Ubuntu VPS
  * Virtual private server with dedicated CPU's running - [Hardware](https://www.netcup.eu/bestellen/produkt.php?produkt=2624)
  * Ubuntu 20.04.3 LTS x86-64 Kernel 5.14.0-051400-generic

* Ubuntu Desktop / Windows Desktop
  * Desktop PC from 2020 - [Hardware](https://pcpartpicker.com/user/JohannesDeml/saved/zz7yK8)
  * Windows 10 Pro x86-64 Build 19043.1266 (21H1/May2021Update)
  * Ubuntu 20.04.3 LTS x86-64 Kernel 5.11.0-37-generic

### Software

* [.NET](https://dotnet.microsoft.com/download/dotnet) 5.0.11 (5.0.1121.47308)
* [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) 0.13.1

### Procedure
For the two desktop setups, the benchmarks are run on a restarted system with 5 minutes idle time before starting the benchmark. They are run with admin privileges and all unnecessary other processes are killed before running the benchmarks. For Ubuntu VPS, the benchmarks are run through continuous integration on a typical indie server setup with other processes running as well. After the benchmarks are run, a list of all running processes to make them more reproducible. To reproduce the benchmarks, run `sudo sh linux-benchmark.sh` or `win-benchmark.bat` . If you want to execute directly from the compiled program, run `./NetworkBenchmarkDotNet -b Essential`.

## Benchmark Results
The raw data and additional files can be downloaded from the [release section](../../releases).

### Benchmark [PingPongUnreliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/UnreliablePerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **1 message** each with the server with **unreliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time (lower is better).
![PingPong Unreliable .NET Benchmark chart](./Docs/nbn-pingpongunreliable-1.0.1.png)

### Benchmark [PingPongReliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/ReliablePerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **1 message** each with the server with **reliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time (lower is better).
![PingPong Reliable .NET Benchmark chart](./Docs/nbn-pingpongreliable-1.0.1.png)

### Benchmark [PingPongBatchedUnreliable](./NetworkBenchmarkDotNet/PredefinedBenchmarks/UnreliablePerformanceBenchmark.cs)

Runs the benchmark with **500** clients, which pingpong **10 messages** each with the server with **unreliable** transmission. The benchmark runs until a total of **500,000** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for multiplexing / message merging performance (higher is better).
![PingPong Batched Unreliable .NET Benchmark chart](./Docs/nbn-pingpongbatchedunreliable-1.0.1.png)

### Benchmark [SampleEchoSimple](./NetworkBenchmarkDotNet/PredefinedBenchmarks/SamplingBenchmark.cs)

Runs the benchmark with **1** client, which pingpong **10 messages** each with the server. The benchmark runs until a total of **100,000** messages are sent to the server and back to the clients. Message size is **128 bytes**.  
This test collects information about generated garbage and CPU times while running the benchmark. Those results can be analyzed with [PerfView](https://github.com/microsoft/perfview) on Windows.

### Overview
This is a comparison between all tests with their message throughput (higher is better).

![Network Benchmark .NET results overview](./Docs/nbn-overview-1.0.1.png)

### Notes

* Ubuntu outperforms windows in almost all tests and libraries. 
* The benchmarks are run locally and therefor a lot of processing power is used for simulating the clients. Therefore, this benchmark does not include a CCU comparison, but is sufficient to compare different libraries for the same setup.
* Also the network latency results do not show any costs of the network itself, but only the cost of the application layer in the round trip time. Since multiple clients are run at the same time, this time with be smaller than presented here.
* Creation, Connection and Disconnection and Disposal of the Server and Clients is not included in the performance benchmarks, but is included in the .nettrace files from the Sampling benchmark.
* To access the Sampling results, you can use [PerfView](https://github.com/microsoft/perfview) to open the `.nettrace` files.
* Kcp2k has been recently added and might have some room for improvements. Especially using `Thread.Sleep` on Windows creates [noticeable delays](https://social.msdn.microsoft.com/Forums/vstudio/en-US/facc2b57-9a27-4049-bb32-ef093fbf4c29/threadsleep1-sleeps-for-156-ms?forum=clr). For now it is excluded of the predefined benchmarks, until its execution and cleanup are improved.
* This is not a general purpose benchmark for every and any game type. Different games have different requirements. I hope these benchmarks help you as a smoke test and give you the possibility to quickly test how your hardware handles the different libraries.


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
  -b, --benchmark                         Run predefined benchmarks [default:
  <All|Custom|Essential|Performance|Qu    Custom]
  ick|Sampling>
  -m, --execution-mode                    Control what parts to run [default:
  <Client|Complete|Server>                Complete]
  -t, --test <Manual|PingPong>            Test type [default: PingPong]
  --transmission <Reliable|Unreliable>    Transmission type [default:
                                          Unreliable]
  -l, --library                           Library target [default: ENet]
  <ENet|Kcp2k|LiteNetLib|NetCoreServer
  >
  -d, --duration <duration>               Test duration in seconds (-1 for
                                          manual stopping) [default: 10]
  --address <address>                     IP Address, can be ipv4 (e.g.
                                          127.0.0.1) or ipv6 (e.g. ::1)
                                          [default: ::1]
  --port <port>                           Socket Port [default: 3330]
  --clients <clients>                     #Simultaneous clients [default: 500]
  --parallel-messages                     #Parallel messages per client
  <parallel-messages>                     [default: 1]
  --message-byte-size                     Message byte size sent by clients
  <message-byte-size>                     [default: 32]
  --message-payload                       Message load sent by clients
  <Ones|Random|Zeros>                     [default: Random]
  --verbose                               Verbose output of test steps and
                                          errors [default: True]
  --client-tick-rate                      Client ticks per second if supported
  <client-tick-rate>                      [default: 60]
  --server-tick-rate                      Server ticks per second if supported
  <server-tick-rate>                      [default: 60]
  --version                               Show version information
  -?, -h, --help                          Show help and usage information
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