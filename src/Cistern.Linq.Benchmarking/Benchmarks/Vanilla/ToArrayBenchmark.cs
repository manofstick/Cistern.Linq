using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |      Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |----------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |  25.42 ns |  0.2742 ns |  0.2565 ns |  0.50 |    0.01 | 0.0076 |     - |     - |      24 B |
    |  SystemLinq |             0 |  51.00 ns |  0.5675 ns |  0.4739 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |  27.63 ns |  0.2382 ns |  0.2228 ns |  0.54 |    0.01 |      - |     - |     - |         - |
    |             |               |           |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |  28.34 ns |  0.2160 ns |  0.2021 ns |  0.39 |    0.00 | 0.0101 |     - |     - |      32 B |
    |  SystemLinq |             1 |  72.45 ns |  0.6857 ns |  0.6414 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |             1 |  47.38 ns |  0.3784 ns |  0.3539 ns |  0.65 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |           |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |  36.58 ns |  0.2633 ns |  0.2334 ns |  0.45 |    0.00 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |            10 |  81.50 ns |  0.4977 ns |  0.4412 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    | CisternLinq |            10 |  55.49 ns |  0.6060 ns |  0.5669 ns |  0.68 |    0.01 | 0.0330 |     - |     - |     104 B |
    |             |               |           |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 881.09 ns |  7.9965 ns |  7.4799 ns |  0.95 |    0.01 | 2.5444 |     - |     - |    8024 B |
    |  SystemLinq |          1000 | 931.90 ns |  9.3684 ns |  8.7632 ns |  1.00 |    0.00 | 2.5444 |     - |     - |    8024 B |
    | CisternLinq |          1000 | 917.19 ns | 15.3088 ns | 11.9521 ns |  0.98 |    0.02 | 2.5444 |     - |     - |    8024 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class ToArrayBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double[] ForLoop()
		{
            var array = new double[Numbers.Length];
            Numbers.CopyTo(array, 0);
			return array;
		}

		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(Numbers);
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Numbers);
		}
	}
}
