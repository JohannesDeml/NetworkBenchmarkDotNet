# .Net Core Network Benchmark

*Low Level .Net Core Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetCoreNetworkBenchmark/all.svg)](../../releases)

## Description

🚧 *Early in development* 🚧

Benchmark for different low level [.Net Core](https://en.wikipedia.org/wiki/.NET_Core) compatible libraries that are compatible with [Unity](https://unity3d.com), but also work as standalone console applications. The benchmark focuses on performance, latency and scalability.

### Supported Libraries

* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 3.0.20)
  * Pure C# / .Net library for TCP/UDP/SSL with no additional protocols on top
  * Packetsize overhead: 0 bytes, but you have to invent the wheel yourself
* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.3)
  * Wrapper for [ENet](https://github.com/lsalzman/enet), building a reliable sequenced protocol on top of UDP
  * Max concurrent connections are limited to 4095 due to the protocol
  * Packetsize overhead: 10 bytes
* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (v 0.9.3.2)
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
Note that the benchmarks run a lot better on Linux compared to Windows  ([Results v 0.1.0](https://github.com/JohannesDeml/NetCoreNetworkBenchmark/releases/tag/0.1.0)) 

[Results](./Benchmarks)


```
OS: Ubuntu 20.04.1 LTS
CPU: Intel® Core™ i5-3570K CPU @ 3.40GHz × 4
Mainboard:  Gigabyte Z77X-D3H Gb LAN (Atheros)
RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)
```

|             | ENet            | LiteNetLib    | NetCoreServer |
| ----------- | --------------- | ------------- | ------------- |
| Benchmark 1 | 197,036 msg/s   | 1,768 msg/s   | 107,772 msg/s |
| Benchmark 2 | 1,908,836 msg/s | 144,585 msg/s | 118,974 msg/s |

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