using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      55.46 ns |     0.8895 ns |     0.8320 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      62.33 ns |     1.0706 ns |     0.8940 ns |  1.12 |    0.02 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     148.01 ns |     2.2688 ns |     2.1223 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     204.09 ns |     3.1720 ns |     2.9671 ns |  1.38 |    0.03 | 0.1044 |     - |     - |     328 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     625.17 ns |    12.4557 ns |    19.3919 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     635.06 ns |    12.3881 ns |    13.7693 ns |  1.02 |    0.04 | 0.1831 |     - |     - |     576 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 157,143.09 ns | 1,465.9700 ns | 1,371.2693 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 115,226.56 ns | 1,415.1445 ns | 1,254.4887 ns |  0.73 |    0.01 | 8.9111 |     - |     - |   28296 B |
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
