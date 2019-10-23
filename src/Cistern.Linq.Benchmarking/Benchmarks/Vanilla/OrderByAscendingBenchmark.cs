using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      52.75 ns |     1.1406 ns |     1.8740 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      60.23 ns |     0.7547 ns |     0.7060 ns |  1.12 |    0.05 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     141.34 ns |     2.1398 ns |     2.0016 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     188.96 ns |     1.9032 ns |     1.6872 ns |  1.34 |    0.02 | 0.0968 |     - |     - |     304 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     456.41 ns |     9.1672 ns |     7.6551 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     457.20 ns |     8.1050 ns |     7.5814 ns |  1.00 |    0.02 | 0.1960 |     - |     - |     616 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 145,233.18 ns | 1,248.0486 ns | 1,167.4254 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 125,193.71 ns | 2,194.9391 ns | 2,053.1474 ns |  0.86 |    0.02 | 8.7891 |     - |     - |   28336 B |
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
