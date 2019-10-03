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
    |  SystemForLoop |             1 |    58.16 ns |  0.2682 ns |  0.2094 ns |  0.99 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternForLoop |             1 |    38.15 ns |  0.1488 ns |  0.1319 ns |  0.65 |    0.00 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |             1 |    58.65 ns |  0.1678 ns |  0.1402 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |             1 |    54.76 ns |  0.1200 ns |  0.1122 ns |  0.93 |    0.00 | 0.0153 |     - |     - |      48 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   116.56 ns |  0.2429 ns |  0.2028 ns |  0.99 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternForLoop |            10 |    99.20 ns |  0.5643 ns |  0.5002 ns |  0.84 |    0.00 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |            10 |   118.31 ns |  0.3474 ns |  0.3250 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |            10 |    80.77 ns |  0.1332 ns |  0.1181 ns |  0.68 |    0.00 | 0.0153 |     - |     - |      48 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 6,613.79 ns | 24.5362 ns | 22.9511 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternForLoop |          1000 | 6,623.09 ns | 17.9169 ns | 15.8829 ns |  1.01 |    0.00 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |          1000 | 6,581.31 ns | 11.3192 ns | 10.0342 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |          1000 | 2,709.63 ns |  8.6806 ns |  7.2487 ns |  0.41 |    0.00 | 0.0153 |     - |     - |      48 B |
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
