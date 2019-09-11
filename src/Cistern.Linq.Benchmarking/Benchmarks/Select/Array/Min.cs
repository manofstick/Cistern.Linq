using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     60.77 ns |  0.4189 ns |  0.3919 ns |  0.94 |    0.01 | 0.0151 |     - |     - |      48 B |
    | CisternForLoop |             1 |     46.08 ns |  0.3616 ns |  0.3382 ns |  0.71 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |     64.72 ns |  0.4780 ns |  0.4471 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |     64.89 ns |  0.5474 ns |  0.5121 ns |  1.00 |    0.01 | 0.0279 |     - |     - |      88 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    161.97 ns |  1.3419 ns |  1.2552 ns |  0.87 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |    139.54 ns |  1.2448 ns |  1.1644 ns |  0.75 |    0.01 | 0.0176 |     - |     - |      56 B |
    |     SystemLinq |            10 |    187.07 ns |  1.5514 ns |  1.3752 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |     88.10 ns |  0.6305 ns |  0.5897 ns |  0.47 |    0.00 | 0.0279 |     - |     - |      88 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  9,582.39 ns | 62.1982 ns | 58.1802 ns |  0.83 |    0.01 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 |  8,798.07 ns | 63.0059 ns | 58.9358 ns |  0.76 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 11,609.24 ns | 78.2910 ns | 73.2334 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 |  2,639.78 ns | 28.4747 ns | 26.6352 ns |  0.23 |    0.00 | 0.0267 |     - |     - |      88 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Min : SelectArrayBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            var min = double.PositiveInfinity;
            var noData = true;
            foreach (var n in SystemNumbers)
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

        [Benchmark]
        public double CisternForLoop()
        {
            var min = double.PositiveInfinity;
            var noData = true;
            foreach (var n in CisternNumbers)
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
		public double SystemLinq() => System.Linq.Enumerable.Min(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(CisternNumbers);
	}
}
