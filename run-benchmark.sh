#!/bin/bash

if [ -z "$1" ]
then
  mode="All"
else 
  mode="$1"
fi

sudo ./bin/NetworkBenchmarkDotNet-Linux/NetworkBenchmarkDotNet -b "$mode"
