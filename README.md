# .Net Core Network Benchmark

*Low Level .Net Core Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetCoreNetworkBenchmark/all.svg)](../../releases)

## Description

NCNB is a benchmark for low level networking libraries using UDP and can be used with [Unity](https://unity3d.com) and for [.Net Core](https://en.wikipedia.org/wiki/.NET_Core) standalone server applications. The benchmark focuses on latency, performance and scalability.

### Supported Libraries

* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.5)
  * Wrapper for [ENet](https://github.com/lsalzman/enet), building a reliable sequenced protocol on top of UDP
  * Max concurrent connections are limited to 4095 due to the protocol
  * Packetsize overhead: 10 bytes
  * [Unity Client Example](https://github.com/JohannesDeml/ENetUnityMobile)
* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (master 252c8eb)
  * Very feature-rich library
  * Packetsize overhead: 1 byte for unreliable, 4 bytes for reliable
  * [Unity Client Example](https://github.com/RevenantX/NetGameExample)
* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 3.0.21)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
  * [Unity Client Example](https://github.com/JohannesDeml/Unity-Net-Core-Networking-Sockets)

## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b All`


``` ini
Results v0.5.0, BenchmarkDotNet v0.12.1, OS ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
```
| Benchmark       | ENet                           | LiteNetLib                     | NetCoreServer                  |
| --------------- | ------------------------------ | ------------------------------ | ------------------------------ |
| **Performance** |                                |                                |                                |
| Performance1    | **192,034 msg/s** (5,207.4 ms) | **78,447 msg/s** (12,747.5 ms) | **109,813 msg/s** (9,106.4 ms) |
| Performance2    | **1,075,153 msg/s** (930.1 ms) | **286,566 msg/s** (3,489.6 ms) | **114,481 msg/s** (8,735.1 ms) |
| **Garbage**     |                                |                                |                                |
| Alloc           | **0.512 MB**                   | **81.161 MB**                  | **161.978 MB**                 |
| GC Pauses       | **83.4 ms** (max 8.4 ms)       | **101.1 ms** (max 2.9 ms)      | **74.5 ms** (max 2.5 ms)       |

![Benchmark Results](./Docs/NetCoreNetworkBenchmark.PerformanceBenchmark-barplot.png)

### Benchmark Performance1
Runs the benchmark with **1,000** clients, which pingpong **1 message** each with the server. The benchmark runs until a total of **1 million** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for getting an idea of an average roundtrip time.

### Benchmark Performance2
Runs the benchmark with **1,000** clients, which pingpong **10 messages** each with the server. The benchmark runs until a total of **1 million** messages are sent to the server and back to the clients. Message size is **32 bytes**.  
This test is for multiplexing / message merging performance.

### Benchmark Garbage

Runs the benchmark with **10** clients, which pingpong **10 messages** each with the server. The benchmark runs until a total of **10,000** messages are sent to the server and back to the clients. Message size is **128 bytes**.  
This test collects information about generated garbage while running the benchmark.

### Notes

* The tests perform very different on Linux compared to Windows 10, since there are a lot of client threads involved and Linux seems to handle them a lot better.
* Creation, Connection and Disconnection and Disposal of the Server and Clients is not included in the performance benchmarks, but is included in the Garbage benchmark.
* Since the clients and the server run on the same machine, there is a lot less network latency as in a real world application. On the other hand, the CPU pressure is a lot higher than for a normal server, since all the clients get there own threads and run on the same machine. Take the results with a grain of salt.
* To access the Garbage results, you can use [PerfView](https://github.com/microsoft/perfview) to open the `.nettrace` files.



## Installation

Make sure you have [.NetCore SDK](https://dotnet.microsoft.com/download) >=3.1 installed.

Then just open the solution file with Visual Studio/Rider/Visual Studio Code and build it. Note that results of the benchmarks can be very different with a different operating system and hardware.

## Usage

```
Usage:
  NetCoreNetworkBenchmark [options]

Options:
  -b, --benchmark <All|Custom|Garbage|Performance>    Run predefined benchmarks [default: Custom]
  -t, --test <PingPong>                               Test type [default: PingPong]
  -l, --library <ENet|LiteNetLib|NetCoreServer>       Library target [default: ENet]
  -d, --duration <duration>                           Test duration in seconds [default: 10]
  --address <address>                                 IP Address, can be ipv4 or ipv6 [default: 127.0.0.1]
  --port <port>                                       Socket Port [default: 3333]
  --clients <clients>                                 # Simultaneous clients [default: 1000]
  --parallel-messages <parallel-messages>             # Parallel messages per client [default: 1]
  --message-byte-size <message-byte-size>             Message byte size sent by clients [default: 32]
  --message-payload <Ones|Random|Zeros>               Message load sent by clients [default: Random]
  --verbose                                           Verbose output of test steps and errors [default: True]
  --client-tick-rate <client-tick-rate>               Client ticks per second if supported [default: 60]
  --server-tick-rate <server-tick-rate>               Server ticks per second if supported [default: 60]
  --version                                           Show version information
  -?, -h, --help                                      Show help and usage information
```

## Contributions are welcome!

Your favorite library is missing, or you feel like the benchmarks are not testing everything relevant? Let's evolve the benchmark together! Either hit me up via [E-mail](mailto:public@deml.io) to discuss your idea, or [open an issue](../../issues), or make a pull request directly. There are a few rules in order to not make the benchmark too cluttered.

### Rules for adding a Library

Your new proposed library ... 

* works with Unity as a Client
* works with .Net Core for the server 
* uses UDP
* is Open Source (can still be commercial)
* is stable enough not to break in the benchmarks
* is interesting/relevant for others

#### How to add a library

1. Add a new folder inside the NetCoreNetworkBenchmark solution with the name of your library
2. Add a script called `YourLibraryBenchmark.cs` which implements [INetworkBenchmark](../../blob/master/NetCoreNetworkBenchmark/INetworkBenchmark.cs)
3. Add your library name to the [NetworkLibrary](../../blob/master/NetCoreNetworkBenchmark/NetworkLibrary.cs) enum
4. Add your Implementation Constructor to `INetworkBenchmark.CreateNetworkBenchmark()`
5. Use the `-l ` argument (or `BenchmarkConfiguration.Library`) to test your library and if everything works as expected.
6. Run the benchmarks `./NetCoreNetworkBenchmark -b All` and see if your library runs correct
7. Create a PR including your benchmark md results 🎉

### Rules for adding a benchmark

Tell us why you think that benchmark is important and what it tests, that the other benchmarks don't do.  
Ideas for benchmarks:

- [x] Benchmark for roundtrip time (Benchmark 1)
- [x] Benchmark for message merging (Benchmark 2)
- [x] Benchmark for garbage generation
- [ ] Benchmark .Net 5.0 once it's stable
- [ ] Benchmark for maximum concurrent clients

## License

[MIT](./LICENSE)
