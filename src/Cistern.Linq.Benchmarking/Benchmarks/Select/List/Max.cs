using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    45.96 ns |   0.6577 ns |   0.6152 ns |  0.89 |    0.02 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    45.72 ns |   0.1802 ns |   0.1505 ns |  0.89 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    51.48 ns |   0.8162 ns |   0.7635 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    79.33 ns |   1.1034 ns |   1.0322 ns |  1.54 |    0.04 | 0.0533 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   131.86 ns |   1.6708 ns |   1.5629 ns |  0.90 |    0.02 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   130.25 ns |   1.8084 ns |   1.6916 ns |  0.89 |    0.02 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   145.91 ns |   2.1478 ns |   2.0091 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   135.23 ns |   1.1092 ns |   1.0376 ns |  0.93 |    0.02 | 0.0532 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,522.59 ns | 113.0372 ns | 105.7351 ns |  0.93 |    0.02 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 8,167.56 ns | 104.3257 ns |  97.5863 ns |  0.89 |    0.02 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 9,184.71 ns | 164.6419 ns | 154.0061 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 5,219.31 ns |  62.5697 ns |  58.5277 ns |  0.57 |    0.01 | 0.0458 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Max : SelectListBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            var noData = true;

            double max = double.NaN;

            using (var e = SystemNumbers.GetEnumerator())
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

        [Benchmark]
        public double CisternForLoop()
        {
            var noData = true;

            double max = double.NaN;

            using (var e = CisternNumbers.GetEnumerator())
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
		public double SystemLinq() => System.Linq.Enumerable.Max(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Max(CisternNumbers);
	}
}
