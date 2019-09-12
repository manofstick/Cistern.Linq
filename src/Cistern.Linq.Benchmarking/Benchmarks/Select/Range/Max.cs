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
    |  SystemForLoop |             1 |     88.04 ns |   1.0323 ns |   0.9656 ns |  1.05 |    0.01 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     71.53 ns |   0.6859 ns |   0.6081 ns |  0.86 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     83.41 ns |   0.9830 ns |   0.8714 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     95.64 ns |   0.9161 ns |   0.8569 ns |  1.15 |    0.01 | 0.0380 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    214.58 ns |   1.0053 ns |   0.9403 ns |  1.02 |    0.01 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    162.37 ns |   2.8981 ns |   2.5691 ns |  0.77 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    211.35 ns |   2.2069 ns |   1.9564 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    121.02 ns |   1.7980 ns |   1.6819 ns |  0.57 |    0.01 | 0.0379 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,170.18 ns | 124.4131 ns | 116.3761 ns |  0.96 |    0.02 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 |  9,478.92 ns | 112.2296 ns | 104.9796 ns |  0.75 |    0.01 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 12,679.82 ns | 211.2398 ns | 197.5939 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  3,346.49 ns |  23.2265 ns |  20.5897 ns |  0.26 |    0.00 | 0.0343 |     - |     - |     120 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Max : SelectRangeBase
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
