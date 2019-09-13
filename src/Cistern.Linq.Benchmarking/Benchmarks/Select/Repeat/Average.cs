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
    |  SystemForLoop |             1 |     75.59 ns |   1.0194 ns |   0.9535 ns |  0.98 |    0.02 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |     63.54 ns |   0.8038 ns |   0.7519 ns |  0.83 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     76.75 ns |   0.7653 ns |   0.7158 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |     96.12 ns |   1.1463 ns |   1.0722 ns |  1.25 |    0.02 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    194.89 ns |   2.1817 ns |   2.0408 ns |  1.03 |    0.02 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    143.94 ns |   1.5547 ns |   1.4542 ns |  0.76 |    0.01 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    188.61 ns |   2.5596 ns |   2.3942 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    111.36 ns |   0.5516 ns |   0.4606 ns |  0.59 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,276.50 ns | 131.7272 ns | 123.2177 ns |  1.11 |    0.02 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  7,357.04 ns | 104.5987 ns |  97.8417 ns |  0.67 |    0.01 | 0.0229 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 11,038.48 ns | 175.3138 ns | 155.4111 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  2,772.77 ns |  53.1275 ns |  59.0510 ns |  0.25 |    0.01 | 0.0420 |     - |     - |     136 B |
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
