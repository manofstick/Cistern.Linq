using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    63.28 ns |  0.5852 ns |  0.5474 ns |  0.94 |    0.01 | 0.0151 |     - |     - |      48 B |
    | CisternForLoop |             1 |    44.84 ns |  0.4035 ns |  0.3774 ns |  0.66 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    67.67 ns |  0.4772 ns |  0.4464 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    66.51 ns |  0.4185 ns |  0.3915 ns |  0.98 |    0.01 | 0.0279 |     - |     - |      88 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   140.18 ns |  0.9934 ns |  0.9292 ns |  0.96 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   115.84 ns |  0.7783 ns |  0.7280 ns |  0.80 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |            10 |   145.54 ns |  1.3537 ns |  1.2663 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |    89.64 ns |  0.7011 ns |  0.6558 ns |  0.62 |    0.01 | 0.0279 |     - |     - |      88 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,238.41 ns | 50.1788 ns | 46.9373 ns |  0.95 |    0.01 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 | 7,501.59 ns | 32.5736 ns | 30.4694 ns |  0.87 |    0.00 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 8,651.65 ns | 38.0758 ns | 35.6161 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 3,317.79 ns | 18.5368 ns | 17.3393 ns |  0.38 |    0.00 | 0.0267 |     - |     - |      88 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Max : SelectArrayBase
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
