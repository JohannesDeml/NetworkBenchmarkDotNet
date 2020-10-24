## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`


```
OS: Ubuntu 20.04.1 LTS
CPU: Intel® Core™ i5-3570K CPU @ 3.40GHz × 4
Mainboard:  Gigabyte Z77X-D3H Gb LAN (Atheros)
RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)
```

### Benchmark 1 (v 0.2.0)
* `./NetCoreNetworkBenchmark -t PingPong -l LiteNetLib -a 127.0.0.1 -p 3333 -c 1000 -s 32 -x Random -m 1 -d 60`
* OS: Linux 5.4.0-52-generic #57-Ubuntu SMP Thu Oct 15 10:57:00 UTC 2020 X64
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
Duration: 60.015 s
Messages sent by clients: 11,826,057
Messages server received: 11,825,081
Messages sent by server: 11,825,080
Messages clients received: 11,825,057

Total data: 360.87 MB
Data throughput: 6.01 MB/s
Message throughput: 197,036 msg/s
Message latency: 5.075 μs
```

#### LiteNetLib
```
Errors: 1

Duration: 60.012 s
Messages sent by clients: 107,098
Messages server received: 106,214
Messages sent by server: 106,214
Messages clients received: 106,098

Total data: 3.24 MB
Data throughput: 0.05 MB/s
Message throughput: 1,768 msg/s
Message latency: 565.627 μs
```

#### NetCoreServer
```
Duration: 60.015 s
Messages sent by clients: 6,468,913
Messages server received: 6,468,037
Messages sent by server: 6,468,037
Messages clients received: 6,467,906

Total data: 197.38 MB
Data throughput: 3.29 MB/s
Message throughput: 107,772 msg/s
Message latency: 9.279 μs
```


### Benchmark 2 (v 0.2.0)
* `./NetCoreNetworkBenchmark -t PingPong -l NetCoreServer -a 127.0.0.1 -p 3333 -c 100 -s 32 -x Random -m 1000 -d 60`
* OS: Linux 5.4.0-52-generic #57-Ubuntu SMP Thu Oct 15 10:57:00 UTC 2020 X64
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
Duration: 60.036 s
Messages sent by clients: 114,699,507
Messages server received: 114,599,508
Messages sent by server: 114,599,507
Messages clients received: 114,599,507

Total data: 3497.30 MB
Data throughput: 58.25 MB/s
Message throughput: 1,908,836 msg/s
Message latency: 0.524 μs
```

#### LiteNetLib
```
Duration: 60.137 s
Messages sent by clients: 8,794,836
Messages server received: 8,697,452
Messages sent by server: 8,697,452
Messages clients received: 8,694,836

Total data: 265.35 MB
Data throughput: 4.41 MB/s
Message throughput: 144,585 msg/s
Message latency: 6.916 μs
```

#### NetCoreServer
```
Duration: 60.686 s
Messages sent by clients: 7,319,602
Messages server received: 7,221,453
Messages sent by server: 7,221,453
Messages clients received: 7,220,046

Total data: 220.34 MB
Data throughput: 3.63 MB/s
Message throughput: 118,974 msg/s
Message latency: 8.405 μs
```
