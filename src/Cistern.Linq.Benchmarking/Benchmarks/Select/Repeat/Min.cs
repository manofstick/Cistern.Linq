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
    |  SystemForLoop |             1 |     84.35 ns |   0.7921 ns |   0.7409 ns |  0.98 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |     69.72 ns |   0.8807 ns |   0.8238 ns |  0.81 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     86.12 ns |   0.3856 ns |   0.3607 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    103.08 ns |   0.4915 ns |   0.4598 ns |  1.20 |    0.01 | 0.0407 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    226.64 ns |   1.6932 ns |   1.5838 ns |  0.93 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    168.65 ns |   1.1731 ns |   1.0974 ns |  0.70 |    0.01 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    242.49 ns |   2.1572 ns |   2.0178 ns |  1.00 |    0.00 | 0.0277 |     - |     - |      88 B |
    |    CisternLinq |            10 |    128.18 ns |   0.8850 ns |   0.7846 ns |  0.53 |    0.00 | 0.0405 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,128.63 ns | 106.8408 ns |  99.9390 ns |  0.89 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  9,171.39 ns |  40.2662 ns |  37.6650 ns |  0.58 |    0.01 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 15,912.07 ns | 166.2018 ns | 155.4653 ns |  1.00 |    0.00 |      - |     - |     - |      88 B |
    |    CisternLinq |          1000 |  3,686.85 ns |  21.6044 ns |  20.2087 ns |  0.23 |    0.00 | 0.0381 |     - |     - |     128 B |
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
