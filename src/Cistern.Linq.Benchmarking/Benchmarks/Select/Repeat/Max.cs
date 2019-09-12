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
    |  SystemForLoop |             1 |     86.27 ns |   0.5844 ns |   0.5467 ns |  0.96 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |     71.41 ns |   0.6297 ns |   0.5890 ns |  0.79 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     89.92 ns |   0.6464 ns |   0.6047 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    103.44 ns |   1.0994 ns |   0.9746 ns |  1.15 |    0.01 | 0.0407 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    232.21 ns |   1.2061 ns |   1.1282 ns |  1.06 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    164.12 ns |   1.2713 ns |   1.1891 ns |  0.75 |    0.01 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    218.54 ns |   2.0109 ns |   1.8810 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    131.55 ns |   0.9954 ns |   0.9311 ns |  0.60 |    0.01 | 0.0405 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,535.56 ns | 122.0148 ns | 114.1327 ns |  1.03 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  8,523.09 ns |  40.7272 ns |  38.0963 ns |  0.65 |    0.01 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 13,160.82 ns | 101.6980 ns |  95.1284 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  4,551.61 ns |  89.8601 ns |  79.6586 ns |  0.35 |    0.01 | 0.0381 |     - |     - |     128 B |
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
