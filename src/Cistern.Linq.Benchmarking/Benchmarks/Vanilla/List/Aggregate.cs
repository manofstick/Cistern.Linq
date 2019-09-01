using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    11.72 ns |  0.0356 ns |  0.0298 ns |  0.58 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    20.04 ns |  0.0592 ns |  0.0553 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    68.76 ns |  0.1548 ns |  0.1448 ns |  3.43 | 0.0228 |     - |     - |      72 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |             1 |    13.85 ns |  0.0528 ns |  0.0494 ns |  0.47 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    29.32 ns |  0.1104 ns |  0.1033 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    73.88 ns |  0.2512 ns |  0.2350 ns |  2.52 | 0.0228 |     - |     - |      72 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |            10 |    32.16 ns |  0.1000 ns |  0.0887 ns |  0.29 |      - |     - |     - |         - |
    |  SystemLinq |            10 |   110.81 ns |  0.2942 ns |  0.2752 ns |  1.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |   110.72 ns |  0.3034 ns |  0.2369 ns |  1.00 | 0.0228 |     - |     - |      72 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |          1000 | 2,175.84 ns |  6.5861 ns |  6.1606 ns |  0.28 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,911.70 ns | 32.1731 ns | 30.0947 ns |  1.00 |      - |     - |     - |      40 B |
    | CisternLinq |          1000 | 3,430.90 ns | 10.3873 ns |  9.2080 ns |  0.43 | 0.0191 |     - |     - |      72 B |
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
