using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     4.068 ns |   0.0699 ns |   0.0654 ns |  0.09 |    0.00 | 0.0076 |     - |     - |      24 B |
    |  SystemLinq |             0 |    47.466 ns |   1.4843 ns |   1.7093 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      24 B |
    | CisternLinq |             0 |    25.432 ns |   0.1981 ns |   0.1853 ns |  0.53 |    0.02 | 0.0076 |     - |     - |      24 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |             1 |     5.444 ns |   0.0921 ns |   0.0862 ns |  0.08 |    0.00 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq |             1 |    64.990 ns |   0.5244 ns |   0.4905 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |             1 |    47.816 ns |   0.3824 ns |   0.3390 ns |  0.74 |    0.01 | 0.0280 |     - |     - |      88 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |            10 |    21.258 ns |   0.1823 ns |   0.1705 ns |  0.13 |    0.00 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |            10 |   162.573 ns |   0.9870 ns |   0.8750 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    | CisternLinq |            10 |   135.624 ns |   1.3164 ns |   1.2313 ns |  0.83 |    0.01 | 0.0508 |     - |     - |     160 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,733.939 ns |   9.4413 ns |   8.8314 ns |  0.19 |    0.00 | 2.5444 |     - |     - |    8024 B |
    |  SystemLinq |          1000 | 9,267.151 ns | 184.9808 ns | 240.5273 ns |  1.00 |    0.00 | 2.5635 |     - |     - |    8072 B |
    | CisternLinq |          1000 | 8,793.908 ns | 195.2214 ns | 411.7882 ns |  0.97 |    0.06 | 2.5635 |     - |     - |    8080 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class SelectBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double[] ForLoop()
		{
			var items = new double[NumberOfItems];
			foreach (var n in Numbers)
			{
				items[(int)n - 1] = n + 1;
			}
			return items;
		}

		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			var items = new double[NumberOfItems];
			foreach (var item in System.Linq.Enumerable.Select(Numbers, n => n + 1))
			{
				items[(int)item - 2] = item;
			}
			return items;
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			var items = new double[NumberOfItems];
			foreach (var item in Enumerable.Select(Numbers, n => n + 1))
			{
				items[(int)item - 2] = item;
			}
			return items;
		}
	}
}
