using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     53.73 ns |  0.7415 ns |  0.6573 ns |  0.94 |    0.02 | 0.0152 |     - |     - |      48 B |
    | CisternForLoop |             1 |     40.30 ns |  0.4797 ns |  0.4487 ns |  0.70 |    0.02 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |     57.33 ns |  0.9422 ns |  0.8813 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    |    CisternLinq |             1 |     75.34 ns |  0.9179 ns |  0.8137 ns |  1.32 |    0.03 | 0.0483 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    144.25 ns |  1.6099 ns |  1.5059 ns |  0.87 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |    122.97 ns |  1.8601 ns |  1.7400 ns |  0.74 |    0.01 | 0.0176 |     - |     - |      56 B |
    |     SystemLinq |            10 |    166.40 ns |  2.3154 ns |  2.1658 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |     99.25 ns |  1.2637 ns |  1.1202 ns |  0.60 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  8,573.90 ns | 95.3061 ns | 89.1493 ns |  0.82 |    0.01 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 |  7,925.16 ns | 75.3580 ns | 70.4899 ns |  0.76 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 10,396.40 ns | 70.7462 ns | 66.1760 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 |  2,900.12 ns | 33.1189 ns | 30.9794 ns |  0.28 |    0.00 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Min : SelectArrayBase
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
