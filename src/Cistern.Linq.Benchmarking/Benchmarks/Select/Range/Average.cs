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
    |  SystemForLoop |             1 |     83.30 ns |   1.0042 ns |   0.9393 ns |  1.03 |    0.02 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     97.26 ns |   1.1518 ns |   1.0210 ns |  1.20 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     80.97 ns |   0.9035 ns |   0.8451 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     86.70 ns |   1.1992 ns |   1.1217 ns |  1.07 |    0.02 | 0.0508 |     - |     - |     160 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    207.89 ns |   2.4722 ns |   2.3125 ns |  1.04 |    0.02 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    194.70 ns |   2.0247 ns |   1.6907 ns |  0.98 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    198.98 ns |   2.5505 ns |   2.3857 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    129.68 ns |   1.9855 ns |   1.8572 ns |  0.65 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,465.53 ns | 149.9186 ns | 140.2340 ns |  0.94 |    0.07 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 |  9,869.45 ns | 115.4295 ns |  96.3889 ns |  0.75 |    0.06 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 14,007.04 ns | 277.8687 ns | 627.1960 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  6,592.26 ns |  71.4728 ns |  66.8557 ns |  0.50 |    0.04 | 0.0458 |     - |     - |     160 B |
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
