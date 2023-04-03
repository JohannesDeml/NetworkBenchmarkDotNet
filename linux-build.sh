#!/bin/bash
# Clean build for Linux

rm -rf ./bin/NetworkBenchmarkDotNet-Linux/

# Options: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
# Build targets: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
dotnet build --configuration Release --framework net6.0 --output ./bin/NetworkBenchmarkDotNet-Linux/
