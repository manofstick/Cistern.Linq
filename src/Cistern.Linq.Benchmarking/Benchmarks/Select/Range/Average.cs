using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     86.82 ns |   1.0175 ns |   0.9518 ns |  1.08 |    0.02 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     70.79 ns |   0.4151 ns |   0.3883 ns |  0.88 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     80.32 ns |   1.0559 ns |   0.9876 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |    109.79 ns |   1.1650 ns |   1.0897 ns |  1.37 |    0.02 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    207.86 ns |   2.7413 ns |   2.5642 ns |  1.03 |    0.02 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    161.13 ns |   1.4428 ns |   1.2048 ns |  0.79 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    202.68 ns |   2.8006 ns |   2.6197 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    151.17 ns |   1.8207 ns |   1.7031 ns |  0.75 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,425.07 ns | 132.6472 ns | 124.0783 ns |  1.05 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 |  9,199.82 ns | 102.9937 ns |  91.3012 ns |  0.77 |    0.01 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 11,873.07 ns | 137.6116 ns | 128.7220 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  4,773.03 ns |  94.6644 ns | 119.7201 ns |  0.40 |    0.01 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Average : SelectRangeBase
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
