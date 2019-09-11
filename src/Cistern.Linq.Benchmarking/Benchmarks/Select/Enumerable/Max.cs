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
    |  SystemForLoop |             1 |     81.30 ns |   0.4378 ns |   0.4095 ns |  0.98 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     78.03 ns |   0.6310 ns |   0.5903 ns |  0.94 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     83.10 ns |   0.5014 ns |   0.4445 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     97.01 ns |   0.8260 ns |   0.7726 ns |  1.17 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    226.31 ns |   1.2444 ns |   1.0391 ns |  0.99 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    213.17 ns |   1.2133 ns |   1.1350 ns |  0.93 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    228.27 ns |   1.5722 ns |   1.4707 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    182.32 ns |   1.0148 ns |   0.9493 ns |  0.80 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,668.83 ns |  87.6355 ns |  77.6866 ns |  1.01 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 13,562.94 ns | 138.2090 ns | 129.2807 ns |  0.93 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 14,593.60 ns |  79.5869 ns |  74.4456 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  8,659.18 ns |  66.4366 ns |  62.1449 ns |  0.59 |    0.01 | 0.0305 |     - |     - |     136 B |
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
