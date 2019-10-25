using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      52.29 ns |     0.7695 ns |     0.6822 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      59.01 ns |     0.9162 ns |     0.8570 ns |  1.13 |    0.02 | 0.0126 |     - |     - |      40 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     143.19 ns |     2.8636 ns |     2.6786 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     167.78 ns |     2.0273 ns |     1.8964 ns |  1.17 |    0.02 | 0.0763 |     - |     - |     240 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     439.14 ns |     8.0067 ns |     7.0978 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     375.92 ns |     5.0944 ns |     4.7653 ns |  0.86 |    0.02 | 0.1554 |     - |     - |     488 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 145,832.02 ns | 1,924.0112 ns | 1,705.5858 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 |  75,517.05 ns |   972.6200 ns |   812.1817 ns |  0.52 |    0.01 | 8.9111 |     - |     - |   28208 B |
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
