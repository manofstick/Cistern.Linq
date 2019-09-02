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
    |     ForLoop |             1 |    21.26 ns |  0.0800 ns |  0.0748 ns |  0.82 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    26.08 ns |  0.0682 ns |  0.0604 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |    61.94 ns |  0.1807 ns |  0.1690 ns |  2.37 |    0.01 | 0.0254 |     - |     - |      80 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    76.04 ns |  0.2297 ns |  0.1918 ns |  0.92 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    82.98 ns |  0.2029 ns |  0.1898 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   114.25 ns |  0.3419 ns |  0.2669 ns |  1.38 |    0.01 | 0.0254 |     - |     - |      80 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,071.37 ns | 16.1726 ns | 15.1279 ns |  0.95 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 6,390.19 ns | 19.0044 ns | 16.8469 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 5,829.13 ns | 19.6987 ns | 18.4262 ns |  0.91 |    0.00 | 0.0229 |     - |     - |      80 B |
    */
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
