## Benchmarks

To reproduce the benchmarks, run `./NetCoreNetworkBenchmark -b`


```
OS: Ubuntu 20.04.1 LTS
CPU: Intel® Core™ i5-3570K CPU @ 3.40GHz × 4
Mainboard:  Gigabyte Z77X-D3H Gb LAN (Atheros)
RAM: G.Skill 16GB (2 x 8 GB) DDR3-1600 (Part number: F3-1600C11-8GNT)
```
### Benchmark 1 (v 0.2.1)
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
Duration: 60.008 s
Messages sent by clients: 11,613,436
Messages server received: 11,612,476
Messages sent by server: 11,612,475
Messages clients received: 11,612,436

Total data: 354.38 MB
Data throughput: 5.91 MB/s
Message throughput: 193,515 msg/s
Message latency: 5.168 μs
```

#### LiteNetLib
```
Duration: 60.092 s
Messages sent by clients: 3,915,508
Messages server received: 3,915,362
Messages sent by server: 3,914,945
Messages clients received: 3,914,508

Total data: 119.46 MB
Data throughput: 1.99 MB/s
Message throughput: 65,142 msg/s
Message latency: 15.351 μs
```

#### NetCoreServer
```
Duration: 60.005 s
Messages sent by clients: 6,604,598
Messages server received: 6,603,735
Messages sent by server: 6,603,733
Messages clients received: 6,603,617

Total data: 201.53 MB
Data throughput: 3.36 MB/s
Message throughput: 110,051 msg/s
Message latency: 9.087 μs
```

### Benchmark 2 (v 0.2.1)
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
Duration: 60.017 s
Messages sent by clients: 129,111,780
Messages server received: 129,011,781
Messages sent by server: 129,011,780
Messages clients received: 129,011,780

Total data: 3937.13 MB
Data throughput: 65.60 MB/s
Message throughput: 2,149,574 msg/s
Message latency: 0.465 μs
```

#### LiteNetLib
```
Duration: 60.012 s
Messages sent by clients: 28,786,912
Messages server received: 28,696,204
Messages sent by server: 28,687,309
Messages clients received: 28,686,912

Total data: 875.46 MB
Data throughput: 14.59 MB/s
Message throughput: 478,019 msg/s
Message latency: 2.092 μs
```

#### NetCoreServer
```
Duration: 60.591 s
Messages sent by clients: 7,296,041
Messages server received: 7,197,985
Messages sent by server: 7,197,984
Messages clients received: 7,196,543

Total data: 219.62 MB
Data throughput: 3.62 MB/s
Message throughput: 118,773 msg/s
Message latency: 8.419 μs
```

