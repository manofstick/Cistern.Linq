using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |        Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |             |             |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    14.38 ns |   0.2250 ns |  0.2104 ns |  0.55 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    26.07 ns |   0.3068 ns |  0.2720 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    37.89 ns |   0.4370 ns |  0.4088 ns |  1.45 |    0.03 | 0.0127 |     - |     - |      40 B |
    |             |               |             |             |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    32.25 ns |   0.2902 ns |  0.2714 ns |  0.36 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    89.01 ns |   1.6500 ns |  1.5434 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    72.07 ns |   0.7078 ns |  0.6275 ns |  0.81 |    0.02 | 0.0126 |     - |     - |      40 B |
    |             |               |             |             |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 2,191.33 ns |  20.0783 ns | 18.7813 ns |  0.33 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 6,670.61 ns | 105.6166 ns | 93.6263 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      40 B |
    | CisternLinq |          1000 | 2,239.97 ns |  36.3307 ns | 32.2062 ns |  0.34 |    0.01 | 0.0114 |     - |     - |      40 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaList_Average : VanillaListBase
    {
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
			}

			return sum / Numbers.Count;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Average(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Average(Numbers);
	}
}
