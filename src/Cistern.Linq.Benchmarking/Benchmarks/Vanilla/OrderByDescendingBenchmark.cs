using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |             0 |      79.26 ns |     0.6300 ns |     0.5893 ns |  1.00 |    0.00 |  0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |     170.28 ns |     1.0029 ns |     0.9381 ns |  2.15 |    0.02 |  0.0660 |     - |     - |     208 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |             1 |     192.99 ns |     1.3284 ns |     1.2426 ns |  1.00 |    0.00 |  0.1042 |     - |     - |     328 B |
    | CisternLinq |             1 |     328.96 ns |     2.0929 ns |     1.9577 ns |  1.70 |    0.02 |  0.1802 |     - |     - |     568 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |            10 |     913.55 ns |     6.2059 ns |     5.8050 ns |  1.00 |    0.00 |  0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |   1,229.80 ns |    13.7040 ns |    12.1483 ns |  1.35 |    0.02 |  0.3605 |     - |     - |    1136 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |          1000 | 112,284.32 ns |   608.0672 ns |   568.7864 ns |  1.00 |    0.00 |  8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 123,859.24 ns | 1,364.7713 ns | 1,276.6079 ns |  1.10 |    0.01 | 14.1602 |     - |     - |   45072 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class OrderByDescendingBenchmark : NumericBenchmarkBase
	{
		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.OrderByDescending(Numbers, n => n));
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Enumerable.OrderByDescending(Numbers, n => n));
		}
	}
}
