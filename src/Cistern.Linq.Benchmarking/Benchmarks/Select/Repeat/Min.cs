using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     84.75 ns |   0.5271 ns |   0.4930 ns |  0.98 |    0.00 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |    111.90 ns |   0.7647 ns |   0.7153 ns |  1.29 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     86.45 ns |   0.4523 ns |   0.4231 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    104.95 ns |   0.6792 ns |   0.5671 ns |  1.21 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    224.97 ns |   1.4770 ns |   1.3816 ns |  0.93 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    226.12 ns |   1.6173 ns |   1.5128 ns |  0.93 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    242.45 ns |   2.0057 ns |   1.8761 ns |  1.00 |    0.00 | 0.0277 |     - |     - |      88 B |
    |    CisternLinq |            10 |    161.02 ns |   1.0821 ns |   1.0122 ns |  0.66 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,144.42 ns |  89.9132 ns |  84.1049 ns |  0.86 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 11,533.48 ns |  76.4010 ns |  67.7275 ns |  0.70 |    0.01 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 16,415.80 ns | 137.9016 ns | 122.2462 ns |  1.00 |    0.00 |      - |     - |     - |      88 B |
    |    CisternLinq |          1000 |  4,055.68 ns |  22.2246 ns |  19.7015 ns |  0.25 |    0.00 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRepeat_Min : SelectRepeatBase
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
