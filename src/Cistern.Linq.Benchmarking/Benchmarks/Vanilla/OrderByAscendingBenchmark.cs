using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |             0 |     79.62 ns |   0.6713 ns |   0.6279 ns |  1.00 |    0.00 |  0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |    170.18 ns |   1.1625 ns |   1.0874 ns |  2.14 |    0.02 |  0.0660 |     - |     - |     208 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |             1 |    187.82 ns |   1.4487 ns |   1.3551 ns |  1.00 |    0.00 |  0.1042 |     - |     - |     328 B |
    | CisternLinq |             1 |    327.15 ns |   1.6914 ns |   1.4994 ns |  1.74 |    0.02 |  0.1802 |     - |     - |     568 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |            10 |    547.83 ns |   6.0503 ns |   5.6595 ns |  1.00 |    0.00 |  0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |    862.75 ns |   7.5711 ns |   7.0820 ns |  1.58 |    0.02 |  0.3605 |     - |     - |    1136 B |
    |             |               |              |             |             |       |         |         |       |       |           |
    |  SystemLinq |          1000 | 69,129.10 ns | 670.6998 ns | 627.3730 ns |  1.00 |    0.00 |  8.9111 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 81,501.80 ns | 555.7784 ns | 519.8755 ns |  1.18 |    0.01 | 14.2822 |     - |     - |   45072 B |
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
