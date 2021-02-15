:: build and run benchmark for windows
:: Options: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
:: Build targets: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog

dotnet build --configuration Release --framework net5.0 --output .\bin\NetworkBenchmarkDotNet-Windows\
.\bin\NetworkBenchmarkDotNet-Windows\NetworkBenchmarkDotNet -b Essential
PAUSE
