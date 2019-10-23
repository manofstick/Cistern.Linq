using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      52.97 ns |     1.1310 ns |     1.7608 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      59.98 ns |     0.8404 ns |     0.7861 ns |  1.13 |    0.04 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     140.91 ns |     1.9046 ns |     1.7816 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     191.44 ns |     3.2888 ns |     3.0763 ns |  1.36 |    0.03 | 0.0992 |     - |     - |     312 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     621.69 ns |     6.4786 ns |     5.7431 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     597.26 ns |     9.1828 ns |     8.5896 ns |  0.96 |    0.02 | 0.1984 |     - |     - |     624 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 152,781.02 ns | 1,700.2350 ns | 1,590.4009 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 133,250.81 ns | 1,825.3818 ns | 1,707.4633 ns |  0.87 |    0.02 | 8.7891 |     - |     - |   28344 B |
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
