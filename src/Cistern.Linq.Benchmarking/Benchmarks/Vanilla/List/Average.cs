using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    14.48 ns |  0.0599 ns |  0.0560 ns |  0.55 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    26.50 ns |  0.0475 ns |  0.0445 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    40.60 ns |  0.0905 ns |  0.0803 ns |  1.53 |    0.00 | 0.0127 |     - |     - |      40 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    32.52 ns |  0.0756 ns |  0.0707 ns |  0.36 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    89.79 ns |  0.1733 ns |  0.1621 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    72.71 ns |  0.1967 ns |  0.1840 ns |  0.81 |    0.00 | 0.0126 |     - |     - |      40 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 2,172.73 ns |  4.5231 ns |  4.2309 ns |  0.32 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 6,712.52 ns | 14.5565 ns | 13.6162 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      40 B |
    | CisternLinq |          1000 | 3,435.70 ns |  7.3276 ns |  6.4957 ns |  0.51 |    0.00 | 0.0114 |     - |     - |      40 B |
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
