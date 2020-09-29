# .Net Core Network Benchmark

*Low Level .Net Core Networking libraries benchmarked for UDP socket performance*

![Screenshot](./Docs/screenshot.png)

[![Releases](https://img.shields.io/github/release/JohannesDeml/NetCoreNetworkBenchmark/all.svg)](../../releases)

## Description

🚧 *Early in development* 🚧

Benchmark for different low level [.Net Core](https://en.wikipedia.org/wiki/.NET_Core) compatible libraries that are compatible with [Unity](https://unity3d.com), but also work as standalone console applications. The benchmark focuses on performance, latency and scalability.

### Supported Libraries

* [NetCoreServer](https://github.com/chronoxor/NetCoreServer) (v 3.0.20)
* [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp) (v 2.4.3)

### Upcoming Libraries

* [LiteNetLib](https://github.com/RevenantX/LiteNetLib) (v 0.9.3.2)

### Todo

- [ ] Test for maximum concurrent clients
- [ ] Test for Garbage generation
- [x] Test for roundtrip time
- [x] Generate simple benchmark statistics

## Benchmarks

Benchmarks are run with an older Ubuntu machine. Surprisingly, with a new Windows machine I got worse results ([Results v 0.1.0](https://github.com/JohannesDeml/NetCoreNetworkBenchmark/releases/tag/0.1.0))  
To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`

* OS: Ubuntu 20.04.1 LTS
* CPU: Intel® Core™ i5-3570K CPU @ **3.40GHz × 4**
* Mainboard:  Gigabyte Z77X-D3H **Gb LAN (Atheros)**
* RAM: G.Skill **16GB** (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)

### Benchmark 1 (v 0.1.0)
* OS: Linux 5.4.0-48-generic #52-Ubuntu SMP Thu Sep 10 10:58:49 UTC 2020 X64
* Framework: .NET Core 3.1.8
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 1000
* Parallel messages per client: 1
* Message size: 32 bytes
* Defined duration: 60 seconds

#### ENet
```
Duration: 60.014 s
Messages sent by clients: 11,753,452
Messages server received: 11,753,455
Messages sent by server: 11,753,455
Messages clients received: 11,753,452

Total data: 358.69 MB
Data throughput: 5.98 MB/s
Message throughput: 195,845 msg/s
Message latency: 5.106 μs
```

#### NetCoreServer
```
Duration: 60.006 s
Messages sent by clients: 6,547,612
Messages server received: 6,546,729
Messages sent by server: 6,546,728
Messages clients received: 6,546,620

Total data: 199.79 MB
Data throughput: 3.33 MB/s
Message throughput: 109,100 msg/s
Message latency: 9.166 μs
```

### Benchmark 2 (v 0.1.0)
* OS: Linux 5.4.0-48-generic #52-Ubuntu SMP Thu Sep 10 10:58:49 UTC 2020 X64
* Framework: .NET Core 3.1.8
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 100
* Parallel messages per client: 1,000
* Message size: 32 bytes
* Defined duration: 60 seconds

#### ENet
```
Duration: 60.009 s
Messages sent by clients: 130,768,872
Messages server received: 130,770,536
Messages sent by server: 130,770,536
Messages clients received: 130,768,872

Total data: 3990.75 MB
Data throughput: 66.50 MB/s
Message throughput: 2,179,168 msg/s
Message latency: 0.459 μs
```

#### NetCoreServer
```
Duration: 60.602 s
Messages sent by clients: 7,222,248
Messages server received: 7,124,206
Messages sent by server: 7,124,205
Messages clients received: 7,123,023

Total data: 217.38 MB
Data throughput: 3.59 MB/s
Message throughput: 117,538 msg/s
Message latency: 8.508 μs
```

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
  -l, --library=VALUE        Library target (Default: ENet)
                               Options: [ENet, NetCoreServer, LiteNetLib]
  -a, --address=VALUE        Address to use (Default: 127.0.0.1)
  -p, --port=VALUE           Port (Default: 3333)
  -c, --clients=VALUE        # Simultaneous clients (Default: 1000)
  -m, --messages=VALUE       # Parallel messages per client (Default: 1)
  -s, --size=VALUE           Message byte size sent by clients (Default: 32)
  -d, --duration=VALUE       Duration fo the test in seconds (Default: 10)
```

## License

[MIT](./LICENSE)