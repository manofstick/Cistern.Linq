using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     1.608 ns |  0.0190 ns |  0.0178 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    22.973 ns |  0.1100 ns |  0.1029 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    33.620 ns |  0.2093 ns |  0.1958 ns |  1.46 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    10.296 ns |  0.0517 ns |  0.0483 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    74.339 ns |  0.2752 ns |  0.2439 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    38.664 ns |  0.2495 ns |  0.2334 ns |  0.52 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   783.662 ns | 15.5524 ns | 26.4091 ns |  0.14 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,518.438 ns | 17.5211 ns | 15.5320 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 |   611.927 ns |  3.7129 ns |  3.4730 ns |  0.11 |    0.00 | 0.0095 |     - |     - |      32 B |
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

            double max = double.NaN;
            var idx = 0;
			for(; idx < Numbers.Length && double.IsNaN(max); ++idx)
			{
				max = Numbers[idx];
			}
            for(; idx < Numbers.Length; ++idx)
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
