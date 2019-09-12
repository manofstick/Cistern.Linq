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
    |  SystemForLoop |             1 |     83.79 ns |   0.7704 ns |   0.7206 ns |  1.05 |    0.01 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     97.73 ns |   0.9531 ns |   0.8915 ns |  1.22 |    0.02 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     80.16 ns |   0.7400 ns |   0.6922 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     90.11 ns |   1.7192 ns |   1.6082 ns |  1.12 |    0.02 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    218.27 ns |   3.1563 ns |   2.9524 ns |  0.96 |    0.01 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    202.05 ns |   2.7135 ns |   2.5382 ns |  0.89 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    225.96 ns |   1.8027 ns |   1.5980 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    141.40 ns |   2.0333 ns |   1.9020 ns |  0.63 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,015.54 ns | 176.1077 ns | 164.7312 ns |  0.72 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 | 10,399.79 ns | 107.7622 ns | 100.8008 ns |  0.58 |    0.01 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 18,049.18 ns | 114.4265 ns |  89.3367 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  4,196.54 ns |  34.7715 ns |  30.8240 ns |  0.23 |    0.00 | 0.0458 |     - |     - |     152 B |
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
