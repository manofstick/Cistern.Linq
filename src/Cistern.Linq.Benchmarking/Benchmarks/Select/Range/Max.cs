using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    60.34 ns |  0.3266 ns |  0.2895 ns |  1.05 |    0.01 | 0.0280 |     - |     - |      88 B |
    | CisternForLoop |             1 |    64.17 ns |  0.7007 ns |  0.6211 ns |  1.11 |    0.01 | 0.0280 |     - |     - |      88 B |
    |     SystemLinq |             1 |    57.62 ns |  0.3066 ns |  0.2868 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |    85.24 ns |  0.3980 ns |  0.3723 ns |  1.48 |    0.01 | 0.0280 |     - |     - |      88 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   124.24 ns |  0.4815 ns |  0.4021 ns |  1.04 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |   144.86 ns |  0.4825 ns |  0.4277 ns |  1.21 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |   119.91 ns |  0.5375 ns |  0.5027 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |   109.01 ns |  0.2864 ns |  0.2392 ns |  0.91 |    0.00 | 0.0280 |     - |     - |      88 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 6,224.39 ns | 16.6687 ns | 14.7764 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 8,135.90 ns | 91.3529 ns | 85.4516 ns |  1.31 |    0.01 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 6,231.74 ns | 11.6133 ns | 10.8631 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      88 B |
    |    CisternLinq |          1000 | 2,851.49 ns | 43.1955 ns | 40.4051 ns |  0.46 |    0.01 | 0.0267 |     - |     - |      88 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Max : SelectRangeBase
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
