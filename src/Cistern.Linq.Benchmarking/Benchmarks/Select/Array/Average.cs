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
    |  SystemForLoop |             1 |    55.27 ns |   0.7214 ns |   0.6748 ns |  0.96 |    0.01 | 0.0152 |     - |     - |      48 B |
    | CisternForLoop |             1 |    40.05 ns |   0.5103 ns |   0.4773 ns |  0.70 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    57.43 ns |   0.1229 ns |   0.1027 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    |    CisternLinq |             1 |    58.61 ns |   0.7796 ns |   0.7292 ns |  1.02 |    0.01 | 0.0304 |     - |     - |      96 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   132.83 ns |   1.7928 ns |   1.6770 ns |  1.06 |    0.02 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   114.46 ns |   2.0821 ns |   1.9476 ns |  0.91 |    0.02 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |            10 |   125.02 ns |   1.2355 ns |   1.0317 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |    77.01 ns |   0.9415 ns |   0.8346 ns |  0.62 |    0.01 | 0.0304 |     - |     - |      96 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 7,340.48 ns |  97.3026 ns |  86.2562 ns |  0.88 |    0.03 | 0.0076 |     - |     - |      48 B |
    | CisternForLoop |          1000 | 6,840.97 ns | 105.0779 ns |  93.1489 ns |  0.82 |    0.03 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 8,360.87 ns | 160.5322 ns | 214.3059 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 2,335.08 ns |  46.0893 ns |  61.5279 ns |  0.28 |    0.01 | 0.0267 |     - |     - |      96 B |
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
