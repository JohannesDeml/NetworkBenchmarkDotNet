## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`


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
