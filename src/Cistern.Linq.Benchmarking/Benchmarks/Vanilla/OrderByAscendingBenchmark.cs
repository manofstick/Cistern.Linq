using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |        Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |      53.99 ns |     1.162 ns |     1.0867 ns |  1.00 |    0.00 | 0.0178 |     - |     - |      56 B |
    | CisternLinq |             0 |      60.02 ns |     1.009 ns |     0.9434 ns |  1.11 |    0.03 | 0.0178 |     - |     - |      56 B |
    |             |               |               |              |               |       |         |        |       |       |           |
    |  SystemLinq |             1 |     146.31 ns |     2.399 ns |     2.2444 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |             1 |     194.88 ns |     2.921 ns |     2.7321 ns |  1.33 |    0.03 | 0.0968 |     - |     - |     304 B |
    |             |               |               |              |               |       |         |        |       |       |           |
    |  SystemLinq |            10 |     456.32 ns |     6.891 ns |     6.1086 ns |  1.00 |    0.00 | 0.2036 |     - |     - |     640 B |
    | CisternLinq |            10 |     426.92 ns |     6.945 ns |     6.4961 ns |  0.93 |    0.01 | 0.1755 |     - |     - |     552 B |
    |             |               |               |              |               |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 150,572.02 ns | 1,120.921 ns | 1,048.5102 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   28360 B |
    | CisternLinq |          1000 |  76,561.91 ns | 1,365.169 ns | 1,276.9797 ns |  0.51 |    0.01 | 8.9111 |     - |     - |   28272 B |
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
