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

[All Benchmarks](./Benchmarks)


```
OS: Ubuntu 20.04.1 LTS
CPU: Intel® Core™ i5-3570K CPU @ 3.40GHz × 4
Mainboard:  Gigabyte Z77X-D3H Gb LAN (Atheros)
RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)
```

### Benchmark 1 (v 0.2.0)
* OS: Linux 5.4.0-51-generic #56-Ubuntu SMP Mon Oct 5 14:28:49 UTC 2020 X64
* Framework: .NET Core 3.1.9
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 1000
* Parallel messages per client: 1
* Message size: 32 bytes
* Message Payload: Random
* Defined duration: 60 seconds

#### ENet
```
Duration: 60.014 s
Messages sent by clients: 11,572,702
Messages server received: 11,571,718
Messages sent by server: 11,571,717
Messages clients received: 11,571,702

Total data: 353.14 MB
Data throughput: 5.88 MB/s
Message throughput: 192,818 msg/s
Message latency: 5.186 μs
```

#### NetCoreServer
```
Duration: 60.006 s
Messages sent by clients: 6,617,639
Messages server received: 6,616,736
Messages sent by server: 6,616,734
Messages clients received: 6,616,624

Total data: 201.92 MB
Data throughput: 3.37 MB/s
Message throughput: 110,267 msg/s
Message latency: 9.069 μs
```

#### LiteNetLib
```
Duration: 60.005 s
Messages sent by clients: 1,001
Messages server received: 62
Messages sent by server: 62
Messages clients received: 1

Total data: 0.00 MB
Data throughput: 0.00 MB/s
Message throughput: 0 msg/s
Message latency: 60005248.400 μs
```

### Benchmark 2 (v 0.2.0)
* OS: Linux 5.4.0-51-generic #56-Ubuntu SMP Mon Oct 5 14:28:49 UTC 2020 X64
* Framework: .NET Core 3.1.9
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 100
* Parallel messages per client: 1,000
* Message size: 32 bytes
* Message Payload: Random
* Defined duration: 60 seconds

#### ENet
```
Duration: 60.024 s
Messages sent by clients: 118,023,191
Messages server received: 117,927,886
Messages sent by server: 117,927,886
Messages clients received: 117,923,191

Total data: 3598.73 MB
Data throughput: 59.96 MB/s
Message throughput: 1,964,608 msg/s
Message latency: 0.509 μs
```

#### NetCoreServer
```
Duration: 61.370 s
Messages sent by clients: 3,061,399
Messages server received: 2,962,852
Messages sent by server: 2,962,851
Messages clients received: 2,961,952

Total data: 90.39 MB
Data throughput: 1.47 MB/s
Message throughput: 48,264 msg/s
Message latency: 20.719 μs
```

#### LiteNetLib
```
Duration: 60.116 s
Messages sent by clients: 9,328,845
Messages server received: 9,232,010
Messages sent by server: 9,232,010
Messages clients received: 9,228,845

Total data: 281.64 MB
Data throughput: 4.68 MB/s
Message throughput: 153,517 msg/s
Message latency: 6.514 μs
```



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