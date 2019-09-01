using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |             1 |    21.21 ns |   0.0510 ns |   0.0477 ns |  0.90 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    23.62 ns |   0.0538 ns |   0.0503 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |   106.18 ns |   0.2440 ns |   0.2282 ns |  4.50 |    0.02 | 0.0381 |     - |     - |     120 B |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |            10 |    72.55 ns |   0.1869 ns |   0.1749 ns |  0.90 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    80.37 ns |   0.1925 ns |   0.1801 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   183.02 ns |   0.3992 ns |   0.3539 ns |  2.28 |    0.01 | 0.0381 |     - |     - |     120 B |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |          1000 | 5,784.50 ns |  16.7659 ns |  14.0003 ns |  0.99 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 5,817.24 ns |  15.9602 ns |  14.1483 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 7,310.23 ns | 141.6345 ns | 145.4482 ns |  1.26 |    0.03 | 0.0305 |     - |     - |     120 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaEnumerable_Average : VanillaEnumerableBase
    {
		[Benchmark]
		public double ForLoop()
		{
            double sum = 0;
            int count = 0;
			foreach (var n in Numbers)
			{
				sum += n;
                count++;
			}

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return sum / count;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Average(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Average(Numbers);
	}
}
