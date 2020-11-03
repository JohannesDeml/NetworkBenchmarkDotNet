``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-QYXZWQ : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Concurrent=False  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  

```
|     Method |       Library |        Mean |     Error |    StdDev |
|----------- |-------------- |------------:|----------:|----------:|
| **Benchmark1** |          **ENet** |  **5,375.7 ms** | **174.01 ms** | **115.10 ms** |
| Benchmark2 |          ENet |    921.5 ms |  29.62 ms |  19.59 ms |
| **Benchmark1** | **NetCoreServer** |  **8,326.7 ms** | **192.85 ms** | **127.56 ms** |
| Benchmark2 | NetCoreServer |  8,943.6 ms |  63.25 ms |  37.64 ms |
| **Benchmark1** |    **LiteNetLib** | **12,763.4 ms** | **437.03 ms** | **289.07 ms** |
| Benchmark2 |    LiteNetLib |  3,556.3 ms |  57.70 ms |  34.34 ms |
