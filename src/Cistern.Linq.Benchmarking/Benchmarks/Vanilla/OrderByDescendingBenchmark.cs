using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      53.18 ns |     0.5661 ns |     0.5295 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      64.32 ns |     1.1024 ns |     1.0312 ns |  1.21 |    0.02 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     139.29 ns |     1.5831 ns |     1.4808 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     202.09 ns |     2.6056 ns |     2.4373 ns |  1.45 |    0.03 | 0.1273 |     - |     - |     400 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     693.04 ns |     5.2439 ns |     4.6486 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     741.59 ns |    12.0043 ns |    11.2288 ns |  1.07 |    0.02 | 0.2270 |     - |     - |     712 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 113,250.39 ns | 1,375.0223 ns | 1,218.9215 ns |  1.00 |    0.00 | 8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 113,542.16 ns | 1,827.9991 ns | 1,709.9115 ns |  1.00 |    0.02 | 9.0332 |     - |     - |   28432 B |
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
