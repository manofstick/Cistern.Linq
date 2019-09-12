using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     86.54 ns |   0.5491 ns |   0.5136 ns |  0.97 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |    116.93 ns |   0.8192 ns |   0.7663 ns |  1.31 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     89.19 ns |   0.6595 ns |   0.6169 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    100.18 ns |   0.5634 ns |   0.4994 ns |  1.12 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    216.81 ns |   1.7835 ns |   1.6682 ns |  1.03 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    218.85 ns |   1.4855 ns |   1.3169 ns |  1.04 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    210.57 ns |   1.5932 ns |   1.4903 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    148.62 ns |   1.1475 ns |   0.8959 ns |  0.71 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,494.11 ns |  86.3370 ns |  80.7597 ns |  1.04 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 11,576.90 ns | 228.8291 ns | 349.4462 ns |  0.88 |    0.04 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 13,001.61 ns | 142.6476 ns | 133.4326 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  5,326.86 ns |  37.4928 ns |  33.2364 ns |  0.41 |    0.00 | 0.0458 |     - |     - |     160 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRepeat_Average : SelectRepeatBase
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
