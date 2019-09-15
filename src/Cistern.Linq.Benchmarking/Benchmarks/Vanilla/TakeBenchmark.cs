using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |    12.47 ns |  0.2307 ns |  0.2158 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    38.31 ns |  0.5251 ns |  0.4912 ns |  3.07 |    0.06 | 0.0152 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |             1 |    12.93 ns |  0.1764 ns |  0.1650 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             1 |    38.95 ns |  0.4803 ns |  0.4493 ns |  3.01 |    0.05 | 0.0152 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |            10 |    79.99 ns |  0.9512 ns |  0.8897 ns |  1.00 |    0.00 | 0.0355 |     - |     - |     112 B |
    | CisternLinq |            10 |    70.90 ns |  0.9444 ns |  0.8372 ns |  0.89 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 2,142.18 ns | 23.6170 ns | 22.0913 ns |  1.00 |    0.00 | 1.2932 |     - |     - |    4072 B |
    | CisternLinq |          1000 |   408.41 ns |  4.5750 ns |  4.0556 ns |  0.19 |    0.00 | 1.3018 |     - |     - |    4096 B |
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
