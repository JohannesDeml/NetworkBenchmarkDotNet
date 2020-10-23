## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`


```
OS: Ubuntu 20.04.1 LTS
CPU: Intel® Core™ i5-3570K CPU @ 3.40GHz × 4
Mainboard:  Gigabyte Z77X-D3H Gb LAN (Atheros)
RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)
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
