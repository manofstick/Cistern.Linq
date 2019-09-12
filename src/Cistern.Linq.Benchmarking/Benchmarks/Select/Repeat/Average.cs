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
    |  SystemForLoop |             1 |     84.74 ns |   0.7314 ns |   0.6841 ns |  0.96 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |     69.77 ns |   0.6084 ns |   0.5691 ns |  0.79 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     88.58 ns |   0.5770 ns |   0.5397 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    116.21 ns |   0.8906 ns |   0.7895 ns |  1.31 |    0.01 | 0.0609 |     - |     - |     192 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    218.51 ns |   1.1085 ns |   1.0369 ns |  1.04 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    159.53 ns |   1.2296 ns |   1.1502 ns |  0.76 |    0.01 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    210.09 ns |   1.6390 ns |   1.5331 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    168.45 ns |   1.3067 ns |   1.1584 ns |  0.80 |    0.01 | 0.0608 |     - |     - |     192 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,502.43 ns | 108.3675 ns | 101.3670 ns |  1.06 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  8,167.81 ns |  48.6060 ns |  40.5882 ns |  0.64 |    0.00 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 12,797.54 ns |  68.1962 ns |  60.4541 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  6,124.68 ns |  49.1017 ns |  45.9298 ns |  0.48 |    0.00 | 0.0534 |     - |     - |     192 B |
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
