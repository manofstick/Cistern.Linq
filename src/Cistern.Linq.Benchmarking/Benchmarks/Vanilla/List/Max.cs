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
    |     ForLoop |             1 |     1.454 ns |  0.0098 ns |  0.0091 ns |  0.05 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    29.132 ns |  0.1184 ns |  0.0989 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    58.473 ns |  0.1350 ns |  0.1263 ns |  2.01 |    0.01 | 0.0178 |     - |     - |      56 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     9.878 ns |  0.0372 ns |  0.0330 ns |  0.11 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    92.384 ns |  0.2348 ns |  0.2196 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    78.775 ns |  0.2167 ns |  0.1921 ns |  0.85 |    0.00 | 0.0178 |     - |     - |      56 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   844.116 ns |  1.6372 ns |  1.4514 ns |  0.12 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,286.886 ns | 20.2352 ns | 16.8973 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      40 B |
    | CisternLinq |          1000 | 2,202.159 ns |  9.1487 ns |  8.1101 ns |  0.30 |    0.00 | 0.0153 |     - |     - |      56 B |
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
