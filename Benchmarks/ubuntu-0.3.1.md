``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-WOXISK : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Concurrent=False  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  

```
|     Method |       Library |        Mean |     Error |    StdDev |
|----------- |-------------- |------------:|----------:|----------:|
| **Benchmark1** |          **ENet** |  **5,270.0 ms** | **171.39 ms** | **101.99 ms** |
| Benchmark2 |          ENet |    908.3 ms |  84.13 ms |  55.64 ms |
| **Benchmark1** | **NetCoreServer** |  **8,826.7 ms** | **272.59 ms** | **180.30 ms** |
| Benchmark2 | NetCoreServer |  9,389.2 ms | 236.10 ms | 140.50 ms |
| **Benchmark1** |    **LiteNetLib** | **13,276.9 ms** | **290.76 ms** | **192.32 ms** |
| Benchmark2 |    LiteNetLib |  2,985.0 ms |  33.39 ms |  22.09 ms |
