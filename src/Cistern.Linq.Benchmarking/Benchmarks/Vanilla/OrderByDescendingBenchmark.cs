using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |             0 |      53.22 ns |     0.6067 ns |     0.5378 ns |  1.00 |    0.00 |  0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      64.31 ns |     1.0171 ns |     0.9514 ns |  1.21 |    0.03 |  0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |             1 |     139.50 ns |     2.0178 ns |     1.8875 ns |  1.00 |    0.00 |  0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     211.18 ns |     4.2890 ns |     4.5892 ns |  1.51 |    0.03 |  0.1376 |     - |     - |     432 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |            10 |     682.34 ns |     6.2674 ns |     5.5559 ns |  1.00 |    0.00 |  0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     804.18 ns |    15.8934 ns |    18.9199 ns |  1.18 |    0.03 |  0.2594 |     - |     - |     816 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |          1000 | 113,124.74 ns | 1,406.2466 ns | 1,246.6009 ns |  1.00 |    0.00 |  8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 103,800.16 ns | 1,559.4475 ns | 1,458.7081 ns |  0.92 |    0.02 | 11.5967 |     - |     - |   36456 B |
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
