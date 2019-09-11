using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    54.72 ns |   0.3849 ns |   0.3601 ns |  0.98 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    56.09 ns |   0.4594 ns |   0.4297 ns |  1.00 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    55.88 ns |   0.3194 ns |   0.2667 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    73.19 ns |   0.4630 ns |   0.4331 ns |  1.31 |    0.01 | 0.0330 |     - |     - |     104 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   156.88 ns |   0.9328 ns |   0.8725 ns |  0.96 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   160.35 ns |   1.3301 ns |   1.1107 ns |  0.98 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   163.35 ns |   0.9576 ns |   0.8957 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   115.13 ns |   0.9404 ns |   0.8796 ns |  0.70 |    0.01 | 0.0330 |     - |     - |     104 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 9,530.15 ns |  80.8022 ns |  75.5824 ns |  0.96 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 9,839.24 ns |  73.7373 ns |  68.9739 ns |  0.99 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 9,938.82 ns | 148.8747 ns | 124.3172 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 4,101.96 ns |  44.5779 ns |  41.6982 ns |  0.41 |    0.01 | 0.0305 |     - |     - |     104 B |
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
