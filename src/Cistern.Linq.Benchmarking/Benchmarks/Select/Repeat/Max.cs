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
    |  SystemForLoop |             1 |     85.25 ns |   0.6877 ns |   0.6433 ns |  0.97 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |    111.25 ns |   0.8783 ns |   0.8216 ns |  1.27 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     87.66 ns |   0.4029 ns |   0.3571 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    103.21 ns |   0.2705 ns |   0.2530 ns |  1.18 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    233.48 ns |   2.0569 ns |   1.9240 ns |  1.07 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    229.20 ns |   1.9170 ns |   1.7931 ns |  1.05 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    218.04 ns |   1.5683 ns |   1.4670 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    166.87 ns |   1.2439 ns |   1.1027 ns |  0.77 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,573.19 ns | 100.4275 ns |  93.9400 ns |  0.95 |    0.02 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 11,579.81 ns | 230.7472 ns | 427.7048 ns |  0.79 |    0.04 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 14,250.18 ns | 268.9470 ns | 264.1419 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  3,944.80 ns |  48.0305 ns |  44.9278 ns |  0.28 |    0.01 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRepeat_Max : SelectRepeatBase
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
