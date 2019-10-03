using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    56.11 ns |  0.1807 ns |  0.1690 ns |  1.02 |    0.00 | 0.0229 |     - |     - |      72 B |
    | CisternForLoop |             1 |    48.29 ns |  0.1708 ns |  0.1598 ns |  0.88 |    0.00 | 0.0229 |     - |     - |      72 B |
    |     SystemLinq |             1 |    55.02 ns |  0.1676 ns |  0.1486 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
    |    CisternLinq |             1 |    60.44 ns |  0.5458 ns |  0.5105 ns |  1.10 |    0.01 | 0.0229 |     - |     - |      72 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   138.87 ns |  0.4211 ns |  0.3733 ns |  0.97 |    0.00 | 0.0229 |     - |     - |      72 B |
    | CisternForLoop |            10 |   140.21 ns |  0.4743 ns |  0.4437 ns |  0.98 |    0.00 | 0.0229 |     - |     - |      72 B |
    |     SystemLinq |            10 |   143.59 ns |  0.2760 ns |  0.2447 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
    |    CisternLinq |            10 |   102.38 ns |  0.4003 ns |  0.3548 ns |  0.71 |    0.00 | 0.0229 |     - |     - |      72 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 7,984.87 ns | 20.6643 ns | 18.3184 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 8,556.19 ns | 17.1232 ns | 16.0171 ns |  1.07 |    0.00 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 7,990.72 ns | 18.6597 ns | 15.5817 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 4,661.95 ns | 63.1526 ns | 59.0729 ns |  0.58 |    0.01 | 0.0229 |     - |     - |      72 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Max : SelectListBase
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
