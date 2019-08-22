﻿using BenchmarkDotNet.Running;
using Cistern.Linq.Benchmarking.Benchmarks;
using System;

namespace Cistern.Linq.Benchmarking
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run<SumBenchmark>();
		}
	}
}
