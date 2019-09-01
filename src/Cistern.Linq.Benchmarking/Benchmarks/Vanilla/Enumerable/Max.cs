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
    |     ForLoop |             1 |    21.39 ns |  0.1390 ns |  0.1161 ns |  0.84 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    25.38 ns |  0.0768 ns |  0.0718 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |   121.35 ns |  0.3713 ns |  0.3474 ns |  4.78 |    0.02 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    76.31 ns |  0.2561 ns |  0.2396 ns |  0.92 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    82.74 ns |  0.2397 ns |  0.2125 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   179.58 ns |  0.5532 ns |  0.4904 ns |  2.17 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,081.20 ns | 18.2512 ns | 16.1792 ns |  0.95 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 6,395.04 ns | 14.8509 ns | 13.8915 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 6,172.89 ns | 12.4414 ns | 11.6377 ns |  0.97 |    0.00 | 0.0381 |     - |     - |     136 B |*/
    [CoreJob, MemoryDiagnoser]
	public class VanillaEnumerable_Max : VanillaEnumerableBase
    {
		[Benchmark]
		public double ForLoop()
		{
            var noData = true;

            double max = double.NaN;

            using (var e = Numbers.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    noData = false;
                    max = e.Current;
                    if (!double.IsNaN(max))
                        break;
                }
                while (e.MoveNext())
                {
                    var n = e.Current;
                    if (n > max)
                        max = n;
                }
            }

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }


            return max;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Max(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Max(Numbers);
	}
}
