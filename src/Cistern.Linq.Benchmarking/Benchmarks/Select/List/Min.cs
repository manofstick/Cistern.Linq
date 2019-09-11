using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     53.14 ns |  0.4774 ns |  0.4466 ns |  0.95 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |     56.75 ns |  0.6130 ns |  0.5734 ns |  1.01 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |     56.11 ns |  0.5978 ns |  0.5592 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |     73.98 ns |  0.4143 ns |  0.3875 ns |  1.32 |    0.02 | 0.0330 |     - |     - |     104 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    167.09 ns |  0.4528 ns |  0.4235 ns |  0.85 |    0.00 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |    168.81 ns |  1.6138 ns |  1.5096 ns |  0.85 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |    197.57 ns |  1.2112 ns |  1.0737 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |    119.27 ns |  0.7846 ns |  0.7339 ns |  0.60 |    0.01 | 0.0329 |     - |     - |     104 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,512.64 ns | 87.6824 ns | 82.0182 ns |  0.79 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 10,869.74 ns | 85.7696 ns | 80.2289 ns |  0.82 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 13,279.41 ns | 66.9004 ns | 59.3055 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  4,501.20 ns | 32.2265 ns | 28.5679 ns |  0.34 |    0.00 | 0.0305 |     - |     - |     104 B |
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
