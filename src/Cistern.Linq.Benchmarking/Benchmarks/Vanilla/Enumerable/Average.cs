using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    21.49 ns |  0.3313 ns |  0.3099 ns |  0.85 |    0.02 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    25.14 ns |  0.3196 ns |  0.2833 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |    65.25 ns |  0.8673 ns |  0.8113 ns |  2.59 |    0.05 | 0.0279 |     - |     - |      88 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    73.47 ns |  0.6931 ns |  0.6144 ns |  0.90 |    0.01 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    81.27 ns |  0.8000 ns |  0.7483 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   119.09 ns |  2.8942 ns |  2.8425 ns |  1.47 |    0.04 | 0.0279 |     - |     - |      88 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 5,752.81 ns | 72.0692 ns | 67.4136 ns |  0.99 |    0.01 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 5,817.98 ns | 70.0666 ns | 62.1122 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 5,563.75 ns | 97.0602 ns | 81.0496 ns |  0.96 |    0.02 | 0.0229 |     - |     - |      88 B |
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
