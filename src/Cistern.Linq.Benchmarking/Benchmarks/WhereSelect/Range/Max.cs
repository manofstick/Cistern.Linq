using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    100.88 ns |   0.6500 ns |  0.5428 ns |  0.97 |    0.01 | 0.0508 |     - |     - |     160 B |
    | CisternForLoop |             1 |     91.05 ns |   0.4579 ns |  0.4283 ns |  0.88 |    0.00 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |    103.93 ns |   0.3043 ns |  0.2846 ns |  1.00 |    0.00 | 0.0508 |     - |     - |     160 B |
    |    CisternLinq |             1 |    125.32 ns |   1.1239 ns |  1.0513 ns |  1.21 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    244.82 ns |   1.8475 ns |  1.7282 ns |  0.98 |    0.01 | 0.0505 |     - |     - |     160 B |
    | CisternForLoop |            10 |    189.91 ns |   1.1064 ns |  1.0349 ns |  0.76 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    250.50 ns |   1.6992 ns |  1.5895 ns |  1.00 |    0.00 | 0.0505 |     - |     - |     160 B |
    |    CisternLinq |            10 |    165.40 ns |   0.9836 ns |  0.9200 ns |  0.66 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,267.43 ns |  52.4528 ns | 49.0643 ns |  0.98 |    0.01 | 0.0458 |     - |     - |     160 B |
    | CisternForLoop |          1000 | 10,217.98 ns |  83.7547 ns | 78.3442 ns |  0.70 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 14,626.06 ns | 110.5403 ns | 92.3062 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     160 B |
    |    CisternLinq |          1000 |  4,892.73 ns |  95.6647 ns | 93.9555 ns |  0.33 |    0.01 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRange_Max : WhereSelectRangeBase
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
