using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    56.13 ns |  0.7773 ns |  0.7271 ns |  0.93 |    0.01 | 0.0152 |     - |     - |      48 B |
    | CisternForLoop |             1 |    39.94 ns |  0.1231 ns |  0.1152 ns |  0.66 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    60.58 ns |  0.9001 ns |  0.8420 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    74.75 ns |  1.1803 ns |  1.1041 ns |  1.23 |    0.02 | 0.0483 |     - |     - |     152 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   124.90 ns |  1.5205 ns |  1.4223 ns |  0.97 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   101.94 ns |  1.2769 ns |  1.1944 ns |  0.79 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |            10 |   128.67 ns |  1.9347 ns |  1.5105 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |   100.00 ns |  1.2551 ns |  1.1741 ns |  0.78 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 7,152.88 ns | 10.2627 ns |  9.0976 ns |  0.92 |    0.01 | 0.0076 |     - |     - |      48 B |
    | CisternForLoop |          1000 | 6,703.03 ns | 78.2556 ns | 73.2003 ns |  0.86 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 7,769.35 ns | 69.4636 ns | 64.9763 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 2,204.57 ns | 31.6520 ns | 29.6073 ns |  0.28 |    0.00 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Max : SelectArrayBase
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
