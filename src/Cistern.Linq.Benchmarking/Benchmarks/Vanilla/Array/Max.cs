using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     1.526 ns |  0.0135 ns |  0.0126 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    22.162 ns |  0.0682 ns |  0.0638 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    26.186 ns |  0.0641 ns |  0.0600 ns |  1.18 |    0.00 | 0.0102 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     9.659 ns |  0.0511 ns |  0.0478 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    70.229 ns |  0.5762 ns |  0.4811 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    31.843 ns |  0.1314 ns |  0.1229 ns |  0.45 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   716.904 ns | 13.8851 ns | 19.0060 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,184.215 ns | 14.7877 ns | 13.1089 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 |   566.121 ns |  2.5112 ns |  2.3489 ns |  0.11 |    0.00 | 0.0095 |     - |     - |      32 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Max : VanillaArrayBase
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
