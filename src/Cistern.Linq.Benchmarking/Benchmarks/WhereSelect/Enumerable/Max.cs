using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     94.95 ns |   0.4877 ns |   0.4562 ns |  0.98 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    109.28 ns |   0.6481 ns |   0.5745 ns |  1.13 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     96.74 ns |   0.5694 ns |   0.5047 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    126.69 ns |   1.0879 ns |   0.9644 ns |  1.31 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    236.86 ns |   2.4464 ns |   2.2884 ns |  0.95 |    0.01 | 0.0529 |     - |     - |     168 B |
    | CisternForLoop |            10 |    251.14 ns |   2.2474 ns |   2.1022 ns |  1.01 |    0.01 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    248.63 ns |   2.5845 ns |   2.4175 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    224.26 ns |   0.7445 ns |   0.6965 ns |  0.90 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,926.36 ns |  79.5277 ns |  74.3903 ns |  0.91 |    0.01 | 0.0458 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 14,734.84 ns |  44.1035 ns |  39.0966 ns |  0.97 |    0.01 | 0.0458 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 15,235.33 ns | 124.8554 ns | 110.6810 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     168 B |
    |    CisternLinq |          1000 |  9,303.43 ns |  89.2148 ns |  69.6531 ns |  0.61 |    0.01 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectEnumerable_Max : WhereSelectEnumerableBase
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
