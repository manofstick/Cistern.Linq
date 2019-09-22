using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    101.56 ns |   0.5126 ns |   0.4795 ns |  1.00 |    0.01 | 0.0508 |     - |     - |     160 B |
    | CisternForLoop |             1 |     92.91 ns |   0.7426 ns |   0.6946 ns |  0.92 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |    101.47 ns |   0.5201 ns |   0.4061 ns |  1.00 |    0.00 | 0.0508 |     - |     - |     160 B |
    |    CisternLinq |             1 |    125.81 ns |   0.9943 ns |   0.9301 ns |  1.24 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    248.57 ns |   2.3350 ns |   2.1842 ns |  0.95 |    0.01 | 0.0505 |     - |     - |     160 B |
    | CisternForLoop |            10 |    197.07 ns |   0.8260 ns |   0.7322 ns |  0.75 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    262.95 ns |   2.0612 ns |   1.9280 ns |  1.00 |    0.00 | 0.0505 |     - |     - |     160 B |
    |    CisternLinq |            10 |    164.95 ns |   1.0077 ns |   0.8414 ns |  0.63 |    0.00 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 15,353.59 ns | 165.2935 ns | 154.6157 ns |  0.90 |    0.01 | 0.0305 |     - |     - |     160 B |
    | CisternForLoop |          1000 | 11,080.38 ns |  90.5808 ns |  84.7293 ns |  0.65 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 17,116.20 ns | 140.9558 ns | 124.9536 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     160 B |
    |    CisternLinq |          1000 |  4,683.89 ns |  55.8260 ns |  52.2197 ns |  0.27 |    0.00 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRange_Min : WhereSelectRangeBase
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
