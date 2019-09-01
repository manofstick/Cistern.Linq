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
    |     ForLoop |             1 |    21.83 ns |  0.1265 ns |  0.1183 ns |  0.90 |    0.01 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    24.37 ns |  0.0559 ns |  0.0523 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |   122.08 ns |  0.2391 ns |  0.2119 ns |  5.01 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    84.41 ns |  0.2741 ns |  0.2564 ns |  0.71 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |   118.97 ns |  0.5133 ns |  0.4801 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    | CisternLinq |            10 |   187.27 ns |  0.6826 ns |  0.6051 ns |  1.57 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,984.04 ns | 12.0916 ns | 11.3105 ns |  0.75 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 9,350.75 ns | 64.4171 ns | 57.1041 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    | CisternLinq |          1000 | 7,094.74 ns | 14.5316 ns | 13.5929 ns |  0.76 |    0.01 | 0.0381 |     - |     - |     136 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaEnumerable_Min : VanillaEnumerableBase
    {
		[Benchmark]
		public double ForLoop()
		{
            var min = double.PositiveInfinity;
            var noData = true;
			foreach (var n in Numbers)
			{
                noData = false;
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

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return min;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Min(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(Numbers);
	}
}
