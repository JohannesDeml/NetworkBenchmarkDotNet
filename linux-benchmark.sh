#!/bin/bash

# build and run benchmark for linux

# Options: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
# Build targets: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
dotnet build --configuration Release --framework net5.0 --output ./bin/NetworkBenchmarkDotNet-Linux/

if [ -z "$1" ]
then
  mode="Essential"
else 
  mode="$1"
fi

./bin/NetworkBenchmarkDotNet-Linux/NetworkBenchmarkDotNet -b "$mode"

echo "--- Benchmark finished ---"
echo "Save current process list"
# Folder should exist, just to be sure create it if it does not
mkdir -p BenchmarkDotNet.Artifacts

ps -aux > ./BenchmarkDotNet.Artifacts/running-processes.txt
ps -e -o %p, -o lstart -o ,%C, -o %mem -o ,%c > ./BenchmarkDotNet.Artifacts/running-processes.csv