using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    13.85 ns |  0.1365 ns |  0.1276 ns |  0.28 |    0.00 | 0.0127 |     - |     - |      40 B |
    |  SystemLinq |             0 |    50.20 ns |  0.3389 ns |  0.3170 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    37.98 ns |  1.6029 ns |  1.8459 ns |  0.76 |    0.04 | 0.0127 |     - |     - |      40 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    15.47 ns |  0.1611 ns |  0.1507 ns |  0.22 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |             1 |    70.99 ns |  0.5615 ns |  0.5253 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    | CisternLinq |             1 |    57.96 ns |  0.6004 ns |  0.5616 ns |  0.82 |    0.01 | 0.0228 |     - |     - |      72 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    42.23 ns |  0.3525 ns |  0.3125 ns |  0.53 |    0.00 | 0.0457 |     - |     - |     144 B |
    |  SystemLinq |            10 |    80.24 ns |  0.5516 ns |  0.5160 ns |  1.00 |    0.00 | 0.0457 |     - |     - |     144 B |
    | CisternLinq |            10 |    66.77 ns |  0.4542 ns |  0.4248 ns |  0.83 |    0.01 | 0.0457 |     - |     - |     144 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 2,467.01 ns | 17.8765 ns | 15.8470 ns |  2.68 |    0.03 | 2.5558 |     - |     - |    8064 B |
    |  SystemLinq |          1000 |   921.45 ns | 11.8489 ns | 10.5037 ns |  1.00 |    0.00 | 2.5568 |     - |     - |    8064 B |
    | CisternLinq |          1000 |   919.77 ns |  8.8981 ns |  7.8880 ns |  1.00 |    0.01 | 2.5568 |     - |     - |    8064 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class ToListBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public List<double> ForLoop()
		{
			var list = new List<double>(NumberOfItems);
			
			foreach (var n in Numbers)
			{
				list.Add(n);
			}

			return list;
		}

		[Benchmark(Baseline = true)]
		public List<double> SystemLinq()
		{
			return System.Linq.Enumerable.ToList(Numbers);
		}
		
		[Benchmark]
		public List<double> CisternLinq()
		{
			return Enumerable.ToList(Numbers);
		}
	}
}
