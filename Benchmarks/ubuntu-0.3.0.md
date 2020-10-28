``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-MXHSPZ : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Concurrent=False  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  

```
|     Method |       Library |        Mean |     Error |    StdDev |
|----------- |-------------- |------------:|----------:|----------:|
| **Benchmark1** |          **ENet** |  **5,304.1 ms** | **120.23 ms** |  **71.55 ms** |
| Benchmark2 |          ENet |    898.7 ms |  77.05 ms |  50.97 ms |
| **Benchmark1** | **NetCoreServer** |  **8,490.2 ms** |  **46.09 ms** |  **30.49 ms** |
| Benchmark2 | NetCoreServer |  8,683.1 ms |  37.25 ms |  24.64 ms |
| **Benchmark1** |    **LiteNetLib** | **13,248.2 ms** | **240.54 ms** | **143.14 ms** |
| Benchmark2 |    LiteNetLib |  2,961.5 ms |  49.71 ms |  32.88 ms |
