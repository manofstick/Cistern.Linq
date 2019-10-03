using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     80.30 ns |  0.3941 ns |  0.3687 ns |  0.99 |    0.01 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             1 |     73.85 ns |  0.2955 ns |  0.2764 ns |  0.91 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             1 |     80.75 ns |  0.5764 ns |  0.5392 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             1 |     90.86 ns |  0.3968 ns |  0.3712 ns |  1.13 |    0.01 | 0.0331 |     - |     - |     104 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    195.92 ns |  0.4773 ns |  0.3727 ns |  1.01 |    0.00 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |            10 |    188.00 ns |  0.5981 ns |  0.5595 ns |  0.97 |    0.00 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |            10 |    194.62 ns |  0.5391 ns |  0.4502 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |            10 |    164.34 ns |  0.7313 ns |  0.6841 ns |  0.84 |    0.00 | 0.0331 |     - |     - |     104 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 11,317.41 ns | 56.0796 ns | 49.7131 ns |  1.03 |    0.00 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 11,571.91 ns | 30.9195 ns | 28.9221 ns |  1.06 |    0.00 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 10,949.52 ns | 32.3345 ns | 28.6637 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  6,582.11 ns | 18.2551 ns | 16.1826 ns |  0.60 |    0.00 | 0.0305 |     - |     - |     104 B |
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
