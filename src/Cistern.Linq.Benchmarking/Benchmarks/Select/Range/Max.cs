using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev |       Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|-------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |           NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |           NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |           NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |           NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |              |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     86.47 ns |   1.6226 ns |   1.5177 ns |     86.24 ns |  1.05 |    0.02 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |    100.40 ns |   2.0446 ns |   4.3572 ns |     98.86 ns |  1.30 |    0.03 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     82.05 ns |   0.8081 ns |   0.7559 ns |     81.84 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     90.90 ns |   1.0111 ns |   0.9458 ns |     91.01 ns |  1.11 |    0.02 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |              |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    218.83 ns |   1.7277 ns |   1.5316 ns |    219.59 ns |  1.05 |    0.01 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    192.74 ns |   2.4559 ns |   2.2972 ns |    192.54 ns |  0.93 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    207.43 ns |   1.1282 ns |   1.0001 ns |    207.76 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    135.03 ns |   1.4742 ns |   1.3790 ns |    135.16 ns |  0.65 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |              |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,325.77 ns | 160.2355 ns | 125.1014 ns | 12,346.77 ns |  0.97 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 |  9,602.98 ns |  47.3668 ns |  44.3069 ns |  9,604.33 ns |  0.75 |    0.01 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 12,747.57 ns | 134.0515 ns | 118.8332 ns | 12,800.57 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  3,989.69 ns |  78.4699 ns | 178.7158 ns |  4,051.27 ns |  0.29 |    0.02 | 0.0458 |     - |     - |     152 B |
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
