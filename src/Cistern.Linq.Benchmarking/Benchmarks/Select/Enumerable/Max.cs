using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     73.23 ns |   1.2112 ns |   1.1330 ns |  0.98 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     70.27 ns |   0.7410 ns |   0.6931 ns |  0.94 |    0.02 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     74.64 ns |   0.9932 ns |   0.9291 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    104.23 ns |   1.5121 ns |   1.4144 ns |  1.40 |    0.03 | 0.0635 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    198.06 ns |   1.9437 ns |   1.8181 ns |  0.97 |    0.02 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    197.85 ns |   2.2135 ns |   2.0705 ns |  0.97 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    204.87 ns |   2.4522 ns |   2.2938 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    197.19 ns |   2.4779 ns |   2.3178 ns |  0.96 |    0.02 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,224.41 ns |  86.7172 ns |  81.1153 ns |  1.01 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,701.36 ns | 162.9560 ns | 152.4291 ns |  0.97 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,106.72 ns |  75.8475 ns |  70.9478 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,489.94 ns | 162.4732 ns | 151.9776 ns |  0.72 |    0.01 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Max : SelectEnumerableBase
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
