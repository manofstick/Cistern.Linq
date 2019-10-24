using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|--------------:|--------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |             0 |      55.23 ns |     0.6810 ns |     0.6371 ns |  1.00 |    0.00 |  0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      90.94 ns |     0.9578 ns |     0.8959 ns |  1.65 |    0.02 |  0.0331 |     - |     - |     104 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |             1 |     145.32 ns |     2.3486 ns |     2.1968 ns |  1.00 |    0.00 |  0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     149.07 ns |     1.8400 ns |     1.7211 ns |  1.03 |    0.02 |  0.0687 |     - |     - |     216 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |            10 |     464.27 ns |     8.0833 ns |     7.5611 ns |  1.00 |    0.00 |  0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     426.41 ns |     4.7830 ns |     4.4740 ns |  0.92 |    0.02 |  0.1602 |     - |     - |     504 B |
    |             |               |               |               |               |       |         |         |       |       |           |
    |  SystemLinq |          1000 | 148,678.19 ns | 2,585.7355 ns | 2,418.6986 ns |  1.00 |    0.00 |  8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 | 106,666.48 ns | 1,394.5083 ns | 1,304.4239 ns |  0.72 |    0.02 | 10.1318 |     - |     - |   32184 B |
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
