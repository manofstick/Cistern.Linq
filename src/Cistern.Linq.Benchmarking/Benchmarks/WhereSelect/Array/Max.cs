using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     93.98 ns |   0.5063 ns |   0.4488 ns |  0.97 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    108.40 ns |   0.4577 ns |   0.3574 ns |  1.11 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     97.33 ns |   0.3986 ns |   0.3729 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    127.52 ns |   1.5701 ns |   1.3918 ns |  1.31 |    0.02 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    236.43 ns |   0.3530 ns |   0.3302 ns |  0.96 |    0.01 | 0.0532 |     - |     - |     168 B |
    | CisternForLoop |            10 |    252.04 ns |   2.7764 ns |   2.5970 ns |  1.02 |    0.02 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    246.94 ns |   2.7363 ns |   2.5595 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    217.35 ns |   1.1197 ns |   0.9926 ns |  0.88 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,911.05 ns |  46.0487 ns |  43.0739 ns |  0.91 |    0.01 | 0.0458 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 14,754.25 ns |  44.2752 ns |  39.2488 ns |  0.97 |    0.01 | 0.0458 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 15,264.46 ns | 156.8278 ns | 146.6968 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     168 B |
    |    CisternLinq |          1000 |  9,310.07 ns |  74.2443 ns |  69.4481 ns |  0.61 |    0.01 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectArray_Max : WhereSelectArrayBase
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
