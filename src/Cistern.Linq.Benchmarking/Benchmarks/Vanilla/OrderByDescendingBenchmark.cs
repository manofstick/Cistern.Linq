using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      52.62 ns |     0.6967 ns |     0.6517 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      65.67 ns |     0.8195 ns |     0.7665 ns |  1.25 |    0.02 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     148.68 ns |     1.0551 ns |     0.9869 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     184.96 ns |     2.6563 ns |     2.2181 ns |  1.24 |    0.02 | 0.0968 |     - |     - |     304 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     639.17 ns |     6.1269 ns |     5.7311 ns |  1.00 |    0.00 | 0.2031 |     - |     - |     640 B |
    | CisternLinq |            10 |     631.01 ns |     6.7745 ns |     6.3369 ns |  0.99 |    0.01 | 0.1955 |     - |     - |     616 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 158,325.88 ns | 2,022.5095 ns | 1,792.9020 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 157,850.18 ns | 1,253.4932 ns | 1,172.5183 ns |  1.00 |    0.01 | 8.7891 |     - |     - |   28336 B |
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
