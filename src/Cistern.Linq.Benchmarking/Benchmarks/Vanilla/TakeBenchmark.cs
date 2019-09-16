using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |    14.24 ns |  0.1764 ns |  0.1563 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    21.61 ns |  0.1226 ns |  0.1087 ns |  1.52 |    0.02 |      - |     - |     - |         - |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |             1 |    14.13 ns |  0.1117 ns |  0.1045 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             1 |    21.63 ns |  0.2750 ns |  0.2296 ns |  1.53 |    0.02 |      - |     - |     - |         - |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |            10 |    87.64 ns |  0.4095 ns |  0.3630 ns |  1.00 |    0.00 | 0.0355 |     - |     - |     112 B |
    | CisternLinq |            10 |    79.21 ns |  0.8627 ns |  0.7648 ns |  0.90 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 2,163.80 ns | 12.8433 ns | 12.0136 ns |  1.00 |    0.00 | 1.2932 |     - |     - |    4072 B |
    | CisternLinq |          1000 |   452.43 ns |  3.1986 ns |  2.9919 ns |  0.21 |    0.00 | 1.3018 |     - |     - |    4096 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class TakeBenchmark : NumericBenchmarkBase
	{
		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Take(Numbers, NumberOfItems / 2));
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Enumerable.Take(Numbers, NumberOfItems / 2));
		}
	}
}
