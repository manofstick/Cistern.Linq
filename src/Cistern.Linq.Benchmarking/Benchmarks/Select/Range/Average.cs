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
    |  SystemForLoop |             1 |     82.56 ns |   0.8246 ns |   0.7714 ns |  1.01 |    0.01 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     70.53 ns |   1.0383 ns |   0.9712 ns |  0.87 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     81.45 ns |   1.1790 ns |   1.1028 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |    103.64 ns |   1.0644 ns |   0.9956 ns |  1.27 |    0.02 | 0.0407 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    209.86 ns |   2.6016 ns |   2.4335 ns |  1.03 |    0.02 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    167.93 ns |   2.5839 ns |   2.4170 ns |  0.83 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    203.00 ns |   2.0711 ns |   1.9373 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    123.04 ns |   2.5781 ns |   3.6142 ns |  0.61 |    0.03 | 0.0405 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,504.15 ns | 137.5339 ns | 128.6493 ns |  1.02 |    0.02 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 |  9,127.95 ns | 115.5752 ns | 102.4544 ns |  0.74 |    0.02 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 12,268.71 ns | 237.8511 ns | 222.4861 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  3,058.32 ns |  59.9527 ns |  85.9824 ns |  0.25 |    0.01 | 0.0381 |     - |     - |     128 B |
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
