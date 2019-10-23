using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      53.53 ns |     0.7335 ns |     0.6861 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      66.47 ns |     0.6237 ns |     0.5834 ns |  1.24 |    0.02 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     144.67 ns |     2.0182 ns |     1.8878 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     179.09 ns |     1.3718 ns |     1.2831 ns |  1.24 |    0.02 | 0.0968 |     - |     - |     304 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     463.50 ns |     8.9011 ns |     8.3261 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     481.39 ns |     5.1763 ns |     4.3225 ns |  1.04 |    0.02 | 0.1955 |     - |     - |     616 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 151,954.07 ns |   804.1755 ns |   712.8807 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 143,228.84 ns | 2,766.8648 ns | 2,717.4312 ns |  0.94 |    0.02 | 8.7891 |     - |     - |   28336 B |
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
