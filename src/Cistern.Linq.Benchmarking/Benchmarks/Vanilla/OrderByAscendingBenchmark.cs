using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |     52.78 ns |   0.7952 ns |   0.7438 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |     65.55 ns |   1.3331 ns |   1.1817 ns |  1.24 |    0.03 | 0.0178 |     - |     - |      56 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |  SystemLinq |             1 |    141.30 ns |   1.9586 ns |   1.6356 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |    203.17 ns |   3.0812 ns |   2.8822 ns |  1.44 |    0.02 | 0.1273 |     - |     - |     400 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |  SystemLinq |            10 |    320.91 ns |   2.7937 ns |   2.6132 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |    395.58 ns |   5.8237 ns |   5.4475 ns |  1.23 |    0.02 | 0.2265 |     - |     - |     712 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 73,771.50 ns | 891.7868 ns | 744.6824 ns |  1.00 |    0.00 | 8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 65,500.02 ns | 734.7518 ns | 687.2873 ns |  0.89 |    0.01 | 9.0332 |     - |     - |   28432 B |
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
