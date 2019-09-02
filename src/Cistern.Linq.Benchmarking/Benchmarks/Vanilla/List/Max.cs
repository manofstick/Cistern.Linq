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
    |     ForLoop |             1 |     1.769 ns |  0.0075 ns |  0.0070 ns |  0.06 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    28.324 ns |  0.0784 ns |  0.0734 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    42.544 ns |  0.1385 ns |  0.1228 ns |  1.50 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    10.610 ns |  0.0325 ns |  0.0288 ns |  0.12 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    92.117 ns |  0.4290 ns |  0.3803 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    62.194 ns |  0.1830 ns |  0.1712 ns |  0.68 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   917.671 ns |  2.2619 ns |  2.0051 ns |  0.13 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,298.489 ns | 68.3326 ns | 57.0608 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      40 B |
    | CisternLinq |          1000 | 2,205.150 ns |  7.1081 ns |  5.9356 ns |  0.30 |    0.00 | 0.0076 |     - |     - |      32 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaList_Max : VanillaListBase
    {
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            double max = double.NaN;
            var idx = 0;
			for(; idx < Numbers.Count && double.IsNaN(max); ++idx)
			{
				max = Numbers[idx];
			}
            for(; idx < Numbers.Count; ++idx)
            {
                var n = Numbers[idx];
                if (n > max)
                    max = n;
            }

            return max;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Max(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Max(Numbers);
	}
}
