using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    46.15 ns |   0.9292 ns |   0.9542 ns |  0.95 |    0.03 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    50.75 ns |   0.6601 ns |   0.5851 ns |  1.04 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    48.83 ns |   0.6522 ns |   0.6101 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    69.06 ns |   0.8466 ns |   0.7920 ns |  1.41 |    0.02 | 0.0355 |     - |     - |     112 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   133.22 ns |   1.5741 ns |   1.4724 ns |  0.92 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   142.19 ns |   1.3879 ns |   1.2983 ns |  0.98 |    0.02 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   145.05 ns |   1.7987 ns |   1.6825 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   101.81 ns |   1.2512 ns |   1.1704 ns |  0.70 |    0.01 | 0.0355 |     - |     - |     112 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,304.54 ns |  88.8485 ns |  83.1089 ns |  0.92 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 8,653.83 ns | 102.7018 ns |  96.0673 ns |  0.95 |    0.02 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 9,073.20 ns |  90.3175 ns |  84.4830 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 3,637.74 ns |  71.9390 ns | 135.1188 ns |  0.39 |    0.01 | 0.0343 |     - |     - |     112 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Average : SelectListBase
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
