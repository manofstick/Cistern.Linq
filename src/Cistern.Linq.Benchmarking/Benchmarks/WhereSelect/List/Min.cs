using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     74.71 ns |  0.6412 ns |  0.5998 ns |  0.83 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             1 |     88.42 ns |  0.5487 ns |  0.4864 ns |  0.99 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     89.77 ns |  0.4152 ns |  0.3883 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    105.85 ns |  0.3896 ns |  0.3454 ns |  1.18 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    190.01 ns |  1.2639 ns |  1.1823 ns |  0.83 |    0.01 | 0.0482 |     - |     - |     152 B |
    | CisternForLoop |            10 |    201.90 ns |  1.1983 ns |  1.0622 ns |  0.88 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    228.19 ns |  0.7900 ns |  0.7390 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    163.41 ns |  1.1605 ns |  1.0288 ns |  0.72 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 11,174.77 ns | 57.3281 ns | 53.6248 ns |  0.81 |    0.01 | 0.0458 |     - |     - |     152 B |
    | CisternForLoop |          1000 | 11,035.68 ns | 52.4058 ns | 49.0204 ns |  0.80 |    0.00 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 13,799.98 ns | 74.3428 ns | 58.0420 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  5,919.05 ns | 57.4777 ns | 53.7647 ns |  0.43 |    0.00 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectList_Min : WhereSelectListBase
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
