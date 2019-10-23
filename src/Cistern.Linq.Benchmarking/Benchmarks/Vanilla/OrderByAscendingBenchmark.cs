using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      54.23 ns |     1.5231 ns |     1.7540 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      59.18 ns |     0.8320 ns |     0.7782 ns |  1.09 |    0.04 | 0.0178 |     - |     - |      56 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     143.68 ns |     2.4086 ns |     1.8805 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     192.60 ns |     2.3256 ns |     2.1754 ns |  1.34 |    0.02 | 0.0994 |     - |     - |     312 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     446.74 ns |     7.5102 ns |     7.0251 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     457.24 ns |     5.3837 ns |     5.0360 ns |  1.02 |    0.02 | 0.1988 |     - |     - |     624 B |
    |             |               |               |               |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 147,078.97 ns | 2,505.9196 ns | 2,344.0388 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 123,668.78 ns | 1,697.4810 ns | 1,587.8248 ns |  0.84 |    0.01 | 8.7891 |     - |     - |   28344 B |
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
