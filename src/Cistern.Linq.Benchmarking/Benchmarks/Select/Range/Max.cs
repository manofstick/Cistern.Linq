using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    63.64 ns |   0.7206 ns |   0.6740 ns |  1.00 |    0.02 | 0.0280 |     - |     - |      88 B |
    | CisternForLoop |             1 |    65.71 ns |   0.9158 ns |   0.8118 ns |  1.03 |    0.02 | 0.0254 |     - |     - |      80 B |
    |     SystemLinq |             1 |    63.83 ns |   1.0721 ns |   0.9504 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |    86.88 ns |   0.5701 ns |   0.5054 ns |  1.36 |    0.02 | 0.0254 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   128.39 ns |   1.8942 ns |   1.7718 ns |  1.01 |    0.03 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |   150.04 ns |   2.1084 ns |   1.9722 ns |  1.18 |    0.02 | 0.0253 |     - |     - |      80 B |
    |     SystemLinq |            10 |   127.35 ns |   2.4194 ns |   2.2632 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |   112.56 ns |   0.7470 ns |   0.6987 ns |  0.88 |    0.01 | 0.0254 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 6,272.76 ns |  50.2427 ns |  46.9970 ns |  0.96 |    0.01 | 0.0229 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 8,551.76 ns | 148.0575 ns | 138.4931 ns |  1.30 |    0.02 | 0.0153 |     - |     - |      80 B |
    |     SystemLinq |          1000 | 6,561.50 ns |  69.2338 ns |  64.7613 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      88 B |
    |    CisternLinq |          1000 | 3,163.38 ns |  61.3727 ns |  77.6168 ns |  0.48 |    0.01 | 0.0229 |     - |     - |      80 B |
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
