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
    |  SystemForLoop |             1 |     88.41 ns |   1.0691 ns |   0.9478 ns |  1.01 |    0.03 | 0.0280 |     - |     - |      88 B |
    | CisternForLoop |             1 |     56.51 ns |   0.9738 ns |   0.9109 ns |  0.64 |    0.02 | 0.0306 |     - |     - |      96 B |
    |     SystemLinq |             1 |     89.45 ns |   1.7775 ns |   2.6055 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |     84.49 ns |   0.2437 ns |   0.2161 ns |  0.96 |    0.03 | 0.0305 |     - |     - |      96 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    200.34 ns |   3.5106 ns |   3.1120 ns |  1.02 |    0.02 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    134.70 ns |   2.6576 ns |   2.4859 ns |  0.69 |    0.02 | 0.0305 |     - |     - |      96 B |
    |     SystemLinq |            10 |    196.64 ns |   3.0757 ns |   2.8770 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    107.20 ns |   0.6540 ns |   0.5797 ns |  0.55 |    0.01 | 0.0305 |     - |     - |      96 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,929.34 ns | 182.0734 ns | 170.3115 ns |  1.07 |    0.02 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  7,172.64 ns | 125.3564 ns | 117.2584 ns |  0.70 |    0.02 | 0.0305 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 10,206.69 ns | 143.9806 ns | 134.6795 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  3,607.70 ns |  44.1313 ns |  41.2804 ns |  0.35 |    0.01 | 0.0305 |     - |     - |      96 B |
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
