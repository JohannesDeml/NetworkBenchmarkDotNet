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

### Linux Setup
* OS: Ubuntu 20.04.1 LTS
* CPU: Intel® Core™ i5-3570K CPU @ **3.40GHz × 4**
* Mainboard:  Gigabyte Z77X-D3H **Gb LAN (Atheros)**
* RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)

### Windows Setup
* OS: Windows 10 Pro 2004 x64 (v10.0.19041)
* CPU: AMD Ryzen 7 3700X **3.6 GHz 8-Core**
* Mainboard: MSI MAG X570 TOMAHAWK WIFI ATX AM4 **Lan: 2.5 Gbit/s**
* RAM: G.Skill Aegis 32 GB (2 x 16 GB) DDR4-3200 CL16
[Complete Build](https://pcpartpicker.com/b/8MMcCJ)

### Benchmark 1 (v 0.1.0)
* OS: Microsoft Windows 10.0.19041 X64
* Framework: .NET Core 3.1.8
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 1000
* Parallel messages per client: 1
* Message size: 32 bytes
* Defined duration: 60 seconds

#### ENet
```
Duration: 60,004 s
Messages sent by clients: 1.584.735
Messages server received: 1.585.734
Messages sent by server: 1.585.734
Messages clients received: 1.584.735

Total data: 48,36 MB
Data throughput: 0,81 MB/s
Message throughput: 26.411 msg/s
Message latency: 37,864 µs
```

#### NetCoreServer
```
Duration: 60,040 s
Messages sent by clients: 4.156.889
Messages server received: 4.156.127
Messages sent by server: 4.156.126
Messages clients received: 4.155.989

Total data: 126,83 MB
Data throughput: 2,11 MB/s
Message throughput: 69.221 msg/s
Message latency: 14,447 µs
```

### Benchmark 2 (v 0.1.0)
* OS: Microsoft Windows 10.0.19041 X64
* Framework: .NET Core 3.1.8
* Test: PingPong
* Address: 127.0.0.1, Port: 3333
* Number of clients: 100
* Parallel messages per client: 1.000
* Message size: 32 bytes
* Defined duration: 60 seconds

#### ENet
```
Duration: 60,015 s
Messages sent by clients: 83.344.879
Messages server received: 83.344.880
Messages sent by server: 83.344.880
Messages clients received: 83.344.879

Total data: 2543,48 MB
Data throughput: 42,38 MB/s
Message throughput: 1.388.738 msg/s
Message latency: 0,720 µs
```

#### NetCoreServer
```
Duration: 60,385 s
Messages sent by clients: 4.451.909
Messages server received: 4.352.375
Messages sent by server: 4.352.374
Messages clients received: 4.352.163

Total data: 132,82 MB
Data throughput: 2,20 MB/s
Message throughput: 72.074 msg/s
Message latency: 13,875 µs
```

## Installation



## License

[MIT](./LICENSE)