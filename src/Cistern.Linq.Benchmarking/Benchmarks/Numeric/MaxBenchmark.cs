using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     0.9256 ns |  0.0140 ns |  0.0131 ns |  0.04 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    22.8922 ns |  0.1329 ns |  0.1243 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    33.9663 ns |  0.2416 ns |  0.2260 ns |  1.48 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     6.3473 ns |  0.0667 ns |  0.0624 ns |  0.09 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    74.4951 ns |  0.5109 ns |  0.4779 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    39.0711 ns |  0.1636 ns |  0.1450 ns |  0.52 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   655.3799 ns |  3.6851 ns |  3.4470 ns |  0.12 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,528.3815 ns | 24.7642 ns | 23.1645 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 |   610.2671 ns |  3.8098 ns |  3.1814 ns |  0.11 |    0.00 | 0.0095 |     - |     - |      32 B |     
    */

    [CoreJob, MemoryDiagnoser]
	public class MaxBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Length == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            double max = 0;
			foreach (var n in Numbers)
			{
				if (n > max)
				{
					max = n;
				}
			}
			
			return max;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Max(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Max(Numbers);
	}
}
