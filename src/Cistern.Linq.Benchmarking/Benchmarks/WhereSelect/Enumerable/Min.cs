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
    |  SystemForLoop |             1 |     93.86 ns |   0.6289 ns |   0.5883 ns |  0.98 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    109.27 ns |   0.4500 ns |   0.4210 ns |  1.14 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     95.77 ns |   1.0871 ns |   0.9078 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    127.13 ns |   1.4996 ns |   1.3294 ns |  1.33 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    245.68 ns |   2.3403 ns |   2.1892 ns |  0.90 |    0.01 | 0.0529 |     - |     - |     168 B |
    | CisternForLoop |            10 |    262.63 ns |   2.1857 ns |   2.0445 ns |  0.97 |    0.02 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    271.42 ns |   3.6925 ns |   3.2733 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    223.58 ns |   0.6275 ns |   0.5869 ns |  0.82 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 15,145.85 ns |  34.5500 ns |  28.8508 ns |  0.84 |    0.01 | 0.0458 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 16,002.03 ns | 130.8002 ns | 122.3506 ns |  0.88 |    0.01 | 0.0305 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 18,121.17 ns | 139.1387 ns | 130.1505 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     168 B |
    |    CisternLinq |          1000 |  8,768.52 ns |  73.9647 ns |  65.5678 ns |  0.48 |    0.00 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectEnumerable_Min : WhereSelectEnumerableBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            var min = double.PositiveInfinity;
            var noData = true;
			foreach (var n in SystemNumbers)
			{
                noData = false;
                if (n < min)
				{
					min = n;
				}
                else if (double.IsNaN(n))
                {
                    min = double.NaN;
                    break;
                }
			}

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return min;
		}

        [Benchmark]
        public double CisternForLoop()
        {
            var min = double.PositiveInfinity;
            var noData = true;
            foreach (var n in CisternNumbers)
            {
                noData = false;
                if (n < min)
                {
                    min = n;
                }
                else if (double.IsNaN(n))
                {
                    min = double.NaN;
                    break;
                }
            }

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return min;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Min(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(CisternNumbers);
	}
}
