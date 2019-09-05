using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     45.57 ns |   0.2647 ns |   0.2067 ns |  0.92 |    0.02 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |     47.59 ns |   0.5902 ns |   0.5521 ns |  0.96 |    0.02 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |     49.62 ns |   0.9902 ns |   0.9262 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |     79.62 ns |   1.2804 ns |   1.1977 ns |  1.61 |    0.05 | 0.0533 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    143.23 ns |   2.0162 ns |   1.8860 ns |  0.84 |    0.02 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |    141.49 ns |   2.1873 ns |   2.0460 ns |  0.83 |    0.02 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |    169.78 ns |   2.0330 ns |   1.9017 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |    138.09 ns |   1.8099 ns |   1.6930 ns |  0.81 |    0.01 | 0.0532 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  9,652.85 ns | 133.7704 ns | 125.1289 ns |  0.81 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 |  9,394.25 ns | 119.8148 ns | 112.0748 ns |  0.79 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 11,895.43 ns | 115.6791 ns | 108.2063 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  5,907.65 ns |  40.5430 ns |  35.9403 ns |  0.50 |    0.01 | 0.0458 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Min : SelectListBase
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
		public double SystemLinq() => System.Linq.Enumerable.Min(CisternNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(CisternNumbers);
	}
}
