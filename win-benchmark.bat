:: build and run benchmark for windows

echo off
Echo --- NBN Predefined Benchmark runner ---
Echo.
Echo Benchmark types: [Quick/Performance/Sampling/Essential]
Echo * Quick (^<1min): Runs a quick benchmark with whatever is set in QuickBenchmark.cs
Echo * Performance (^>15min): High Performance statistical test with all included libraries
Echo * Sampling (^<1min): Test with all included libraries using cpu sampling and memory allocation statistics
Echo * Essential (^>15min): Running Performance + Sampling Benchmark
Echo * Custom: Use the commandline with .\NetworkBenchmarkDotNet --help to see how to use it
Echo.
set benchmark=Essential
set /p benchmark=Which benchmark do you want to run [Quick/Performance/Sampling/Essential] (default - %benchmark%)?:

echo on
:: Options: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
:: Build targets: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
dotnet build --configuration Release --framework net5.0 --output .\bin\NetworkBenchmarkDotNet-Windows\
.\bin\NetworkBenchmarkDotNet-Windows\NetworkBenchmarkDotNet -b %benchmark%

echo off
Echo --- Benchmarks finished ---
Echo Save current process list
:: Folder should exist, just to be sure create it if it does not
if not exist "BenchmarkDotNet.Artifacts" mkdir BenchmarkDotNet.Artifacts

echo on
:: Store currently running processes
tasklist /V /FO CSV > "BenchmarkDotNet.Artifacts\running-processes.csv"
tasklist /V > "BenchmarkDotNet.Artifacts\running-processes.txt"

PAUSE
