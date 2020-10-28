# .Net Core Network Benchmark

*Low Level .Net Core Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetCoreNetworkBenchmark/all.svg)](../../releases)

## Description

Benchmark for different low level [.Net Core](https://en.wikipedia.org/wiki/.NET_Core) compatible libraries that are compatible with [Unity](https://unity3d.com), but also work as standalone console applications. The benchmark focuses on performance, latency and scalability.

### Supported Libraries

* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 3.0.20)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.3)
  * Wrapper for [ENet](https://github.com/lsalzman/enet), building a reliable sequenced protocol on top of UDP
  * Max concurrent connections are limited to 4095 due to the protocol
  * Packetsize overhead: 10 bytes
* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (master 252c8eb)
  * Very feature-rich library
  * Packetsize overhead: 1 byte for unreliable, 4 bytes for reliable

### Todo

- [ ] Test for maximum concurrent clients
- [ ] Test for Garbage generation
- [ ] Test on local network, instead of one machine
- [x] Test for roundtrip time (Benchmark 1)
- [x] Generate simple benchmark statistics

## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`

[All Results](./Benchmarks)


``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-MXHSPZ : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Concurrent=False  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  

```
|     Method |       Library |        Mean |     Error |    StdDev |
|----------- |-------------- |------------:|----------:|----------:|
| **Benchmark1** |          **ENet** |  **5,304.1 ms** | **120.23 ms** |  **71.55 ms** |
| Benchmark2 |          ENet |    898.7 ms |  77.05 ms |  50.97 ms |
| **Benchmark1** | **NetCoreServer** |  **8,490.2 ms** |  **46.09 ms** |  **30.49 ms** |
| Benchmark2 | NetCoreServer |  8,683.1 ms |  37.25 ms |  24.64 ms |
| **Benchmark1** |    **LiteNetLib** | **13,248.2 ms** | **240.54 ms** | **143.14 ms** |
| Benchmark2 |    LiteNetLib |  2,961.5 ms |  49.71 ms |  32.88 ms |

![Benchmark Results](./Docs/benchmark.png)

### Notes

* The Benchmark runs the pingpong test with **1,000** clients, which pingpong **1 message** each with the server. The benchmark runs until a total of 1m messages are sent to the server and back to the clients. Message size is 32 bytes.
* The tests perform very different on Linux compared to Windows 10, since there are a lot of client threads involved and Linux seems to handle them a lot better.
* Since the clients and the server run on the same machine, there is a lot less network latency as in a real world application. On the other hand, the CPU pressure is a lot higher than for a normal server, since all the clients get there own threads and run on the same machine. Take the results with a grain of salt.

## Installation

Make sure you have [.NetCore SDK](https://dotnet.microsoft.com/download) (>=3.1 recommended) installed.

Then just open the solution file with Visual Studio/Rider/Visual Studio Code and build it.

## Usage

```
  -h, -?, --help             Show help
  -b, --benchmark            Run predefined full benchmark with all tests and 
                               libraries, ignores all other settings
  -t, --test=VALUE           Test (Default: PingPong)
                               Options: [PingPong]
  -l, --library=VALUE        Library target (Default: LiteNetLib)
                               Options: [ENet, NetCoreServer, LiteNetLib]
  -a, --address=VALUE        Address to use (Default: 127.0.0.1)
  -p, --port=VALUE           Port (Default: 3333)
  -c, --clients=VALUE        # Simultaneous clients (Default: 1000)
  -m, --messages=VALUE       # Parallel messages per client (Default: 1)
  -s, --size=VALUE           Message byte size sent by clients (Default: 32)
  -x, --messageload=VALUE    Message load sent by clients (Default: Ones)
                               Options: [Random, Zeros, Ones]
  -d, --duration=VALUE       Duration fo the test in seconds (Default: 10)
```

## License

[MIT](./LICENSE)