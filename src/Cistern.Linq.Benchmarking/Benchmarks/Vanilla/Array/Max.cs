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
    |     ForLoop |             1 |     1.566 ns |  0.0251 ns |  0.0235 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    23.187 ns |  0.1333 ns |  0.1247 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    29.445 ns |  0.1551 ns |  0.1451 ns |  1.27 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    10.382 ns |  0.0557 ns |  0.0494 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    76.118 ns |  0.5289 ns |  0.4689 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    36.100 ns |  0.2479 ns |  0.2319 ns |  0.47 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 |   797.591 ns | 16.0685 ns | 29.7839 ns |  0.14 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,597.465 ns | 45.8056 ns | 42.8466 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 |   358.737 ns |  1.6581 ns |  1.5510 ns |  0.06 |    0.00 | 0.0100 |     - |     - |      32 B |
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
