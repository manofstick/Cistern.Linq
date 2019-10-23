using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      52.97 ns |     0.9539 ns |     0.8923 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      62.62 ns |     0.9743 ns |     0.9113 ns |  1.18 |    0.03 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     143.29 ns |     2.2967 ns |     2.1483 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     192.93 ns |     2.0062 ns |     1.8766 ns |  1.35 |    0.02 | 0.0968 |     - |     - |     304 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     590.97 ns |     7.6297 ns |     7.1368 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     635.98 ns |     7.3114 ns |     6.8391 ns |  1.08 |    0.02 | 0.1955 |     - |     - |     616 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 153,244.61 ns | 2,154.1025 ns | 2,014.9489 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 140,530.78 ns | 1,556.4623 ns | 1,455.9158 ns |  0.92 |    0.01 | 8.7891 |     - |     - |   28336 B |
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
