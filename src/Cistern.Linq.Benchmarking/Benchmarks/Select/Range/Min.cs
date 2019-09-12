using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     83.13 ns |   1.1153 ns |   1.0433 ns |  1.04 |    0.02 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     70.58 ns |   0.9741 ns |   0.9111 ns |  0.89 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     79.67 ns |   1.1871 ns |   1.1104 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     96.27 ns |   0.7204 ns |   0.6738 ns |  1.21 |    0.02 | 0.0380 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    224.01 ns |   1.5008 ns |   1.4038 ns |  0.98 |    0.01 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    169.96 ns |   1.2278 ns |   1.0884 ns |  0.74 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    229.13 ns |   1.9097 ns |   1.7864 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    123.85 ns |   1.1539 ns |   1.0794 ns |  0.54 |    0.01 | 0.0379 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,085.18 ns | 139.5711 ns | 130.5549 ns |  0.83 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 | 10,095.54 ns | 109.1157 ns | 102.0669 ns |  0.64 |    0.01 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 15,812.97 ns | 257.8060 ns | 241.1519 ns |  1.00 |    0.00 |      - |     - |     - |      96 B |
    |    CisternLinq |          1000 |  4,123.74 ns |  81.7892 ns | 106.3491 ns |  0.26 |    0.01 | 0.0343 |     - |     - |     120 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Min : SelectRangeBase
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
