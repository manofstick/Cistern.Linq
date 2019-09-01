using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     15.30 ns |  0.0509 ns |  0.0451 ns |  0.55 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |     27.74 ns |  0.1011 ns |  0.0896 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |     58.05 ns |  0.2741 ns |  0.2564 ns |  2.09 |    0.01 | 0.0178 |     - |     - |      56 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     42.33 ns |  0.1064 ns |  0.0996 ns |  0.33 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    127.07 ns |  0.3440 ns |  0.3218 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |     89.79 ns |  0.1282 ns |  0.1137 ns |  0.71 |    0.00 | 0.0178 |     - |     - |      56 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |  3,051.55 ns |  5.9297 ns |  5.2566 ns |  0.30 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 10,150.62 ns | 19.5361 ns | 17.3182 ns |  1.00 |    0.00 |      - |     - |     - |      40 B |
    | CisternLinq |          1000 |  3,108.63 ns |  6.7272 ns |  6.2926 ns |  0.31 |    0.00 | 0.0153 |     - |     - |      56 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaListe_Min : VanillaListBase
    {
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            double min = 0;
			foreach (var n in Numbers)
			{
				if (n < min)
				{
					min = n;
				}
                else if (double.IsNaN(n))
                {
                    min = double.NaN;
                    break;
                }
			}
			
			return min;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Min(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(Numbers);
	}
}
