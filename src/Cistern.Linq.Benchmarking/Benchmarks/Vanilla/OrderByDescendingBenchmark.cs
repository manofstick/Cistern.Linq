using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      51.64 ns |     0.9158 ns |     0.8566 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      77.47 ns |     0.8862 ns |     0.8289 ns |  1.50 |    0.03 | 0.0356 |     - |     - |     112 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     142.17 ns |     1.9787 ns |     1.7541 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     193.85 ns |     2.8489 ns |     2.6649 ns |  1.37 |    0.03 | 0.0992 |     - |     - |     312 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     609.86 ns |     4.3101 ns |     4.0317 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     586.87 ns |     7.2521 ns |     6.7836 ns |  0.96 |    0.01 | 0.1984 |     - |     - |     624 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 151,807.11 ns | 1,642.5118 ns | 1,536.4065 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 134,368.80 ns | 2,421.4219 ns | 2,264.9996 ns |  0.89 |    0.02 | 8.7891 |     - |     - |   28344 B |
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
