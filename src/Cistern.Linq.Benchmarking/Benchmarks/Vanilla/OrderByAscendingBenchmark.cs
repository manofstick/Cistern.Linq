using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |             0 |     53.18 ns |   0.5987 ns |   0.5601 ns |  1.00 |    0.00 |  0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |     64.26 ns |   0.5131 ns |   0.4549 ns |  1.21 |    0.02 |  0.0178 |     - |     - |      56 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |             1 |    139.36 ns |   2.0147 ns |   1.8845 ns |  1.00 |    0.00 |  0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |    208.62 ns |   2.8403 ns |   2.6568 ns |  1.50 |    0.02 |  0.1376 |     - |     - |     432 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |            10 |    319.64 ns |   4.0156 ns |   3.7562 ns |  1.00 |    0.00 |  0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |    401.95 ns |   4.7639 ns |   4.2231 ns |  1.26 |    0.02 |  0.2599 |     - |     - |     816 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |          1000 | 71,615.09 ns | 884.0836 ns | 826.9723 ns |  1.00 |    0.00 |  8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 65,952.84 ns | 945.0235 ns | 883.9756 ns |  0.92 |    0.02 | 11.5967 |     - |     - |   36456 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class OrderByAscendingBenchmark : NumericBenchmarkBase
	{
		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.OrderBy(Numbers, n => n));
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Enumerable.OrderBy(Numbers, n => n));
		}
	}
}
