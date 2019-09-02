using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    11.45 ns |  0.0344 ns |  0.0322 ns |  0.55 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    20.88 ns |  0.0650 ns |  0.0608 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    55.01 ns |  0.1568 ns |  0.1466 ns |  2.63 | 0.0152 |     - |     - |      48 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |             1 |    14.05 ns |  0.0877 ns |  0.0821 ns |  0.50 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    28.05 ns |  0.1373 ns |  0.1284 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    61.31 ns |  0.1964 ns |  0.1837 ns |  2.19 | 0.0151 |     - |     - |      48 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |            10 |    31.90 ns |  0.2033 ns |  0.1698 ns |  0.28 |      - |     - |     - |         - |
    |  SystemLinq |            10 |   114.77 ns |  0.1618 ns |  0.1514 ns |  1.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    91.34 ns |  0.2370 ns |  0.2217 ns |  0.80 | 0.0151 |     - |     - |      48 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |          1000 | 2,176.02 ns |  7.8254 ns |  7.3199 ns |  0.26 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 8,525.36 ns | 36.5001 ns | 32.3564 ns |  1.00 |      - |     - |     - |      40 B |
    | CisternLinq |          1000 | 3,414.33 ns |  7.2863 ns |  6.8156 ns |  0.40 | 0.0114 |     - |     - |      48 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaList_Aggreate : VanillaListBase
    {
		[Benchmark]
		public double ForLoop()
		{
			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
			}
			return sum;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(Numbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(Numbers, 0.0, (a, c) => a + c);
    }
}
