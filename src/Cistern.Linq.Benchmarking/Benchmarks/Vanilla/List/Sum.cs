using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    11.72 ns |  0.0495 ns |  0.0463 ns |  0.62 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    18.93 ns |  0.0642 ns |  0.0600 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    55.89 ns |  0.1344 ns |  0.1257 ns |  2.95 | 0.0178 |     - |     - |      56 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |             1 |    13.87 ns |  0.0412 ns |  0.0365 ns |  0.53 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    25.97 ns |  0.0504 ns |  0.0472 ns |  1.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    57.75 ns |  0.1574 ns |  0.1229 ns |  2.22 | 0.0178 |     - |     - |      56 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |            10 |    32.09 ns |  0.1192 ns |  0.1115 ns |  0.32 |      - |     - |     - |         - |
    |  SystemLinq |            10 |   101.34 ns |  0.3341 ns |  0.3125 ns |  1.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    78.12 ns |  0.1559 ns |  0.1382 ns |  0.77 | 0.0178 |     - |     - |      56 B |
    |             |               |             |            |            |       |        |       |       |           |
    |     ForLoop |          1000 | 2,167.15 ns |  7.2741 ns |  6.8042 ns |  0.32 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 6,837.72 ns | 21.7689 ns | 20.3627 ns |  1.00 | 0.0076 |     - |     - |      40 B |
    | CisternLinq |          1000 | 2,194.41 ns |  7.0532 ns |  5.8897 ns |  0.32 | 0.0153 |     - |     - |      56 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaList_Sum : VanillaListBase
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
		public double SystemLinq() => System.Linq.Enumerable.Sum(Numbers);
		
		[Benchmark]
		public double CisternLinq()  => Cistern.Linq.Enumerable.Sum(Numbers);
	}
}
