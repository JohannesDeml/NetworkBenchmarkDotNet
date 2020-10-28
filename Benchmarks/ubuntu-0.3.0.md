``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-CUQNUB : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Concurrent=False  Server=True  InvocationCount=1  
IterationCount=10  LaunchCount=1  UnrollFactor=1  
WarmupCount=1  

```
|       Method |       Library | MessageTarget |     Mean |    Error |   StdDev |
|------------- |-------------- |-------------- |---------:|---------:|---------:|
| **RunBenchmark** |          **ENet** |       **1000000** |  **5.301 s** | **0.0580 s** | **0.0345 s** |
| **RunBenchmark** | **NetCoreServer** |       **1000000** |  **8.855 s** | **0.0239 s** | **0.0158 s** |
| **RunBenchmark** |    **LiteNetLib** |       **1000000** | **13.368 s** | **0.4677 s** | **0.3094 s** |
