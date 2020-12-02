#!/bin/bash

if [ -z "$1" ]
then
  mode="All"
else 
  mode="$1"
fi

./bin/NetworkingBenchmarkDotNet-Linux/NetworkingBenchmarkDotNet -b "$mode"
