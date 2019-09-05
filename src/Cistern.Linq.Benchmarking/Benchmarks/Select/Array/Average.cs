using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    54.83 ns |   0.7966 ns |   0.7451 ns |  1.00 |    0.01 | 0.0152 |     - |     - |      48 B |
    | CisternForLoop |             1 |    39.19 ns |   0.4711 ns |   0.4407 ns |  0.71 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    55.02 ns |   0.5770 ns |   0.5115 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    |    CisternLinq |             1 |    65.61 ns |   0.8974 ns |   0.8394 ns |  1.19 |    0.02 | 0.0508 |     - |     - |     160 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   132.24 ns |   1.3286 ns |   1.2428 ns |  1.06 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   114.97 ns |   0.2083 ns |   0.1740 ns |  0.93 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |            10 |   124.25 ns |   1.5818 ns |   1.4023 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |   108.15 ns |   1.4346 ns |   1.3419 ns |  0.87 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 7,233.12 ns | 107.6372 ns | 100.6839 ns |  0.96 |    0.01 | 0.0076 |     - |     - |      48 B |
    | CisternForLoop |          1000 | 6,661.71 ns | 100.7873 ns |  94.2765 ns |  0.88 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 7,536.91 ns |  96.1366 ns |  89.9262 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    |    CisternLinq |          1000 | 4,194.02 ns |  55.9685 ns |  52.3530 ns |  0.56 |    0.01 | 0.0458 |     - |     - |     160 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Average : SelectArrayBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            double sum = 0;
            int count = 0;
            foreach (var n in SystemNumbers)
            {
                sum += n;
                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return sum / count;
        }

        [Benchmark]
        public double CisternForLoop()
        {
            double sum = 0;
            int count = 0;
            foreach (var n in CisternNumbers)
            {
                sum += n;
                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return sum / count;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Average(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Average(CisternNumbers);
	}
}
