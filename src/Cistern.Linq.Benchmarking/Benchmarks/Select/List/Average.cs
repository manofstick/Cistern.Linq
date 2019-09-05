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
    |  SystemForLoop |             1 |    45.66 ns |   0.3618 ns |   0.3384 ns |  0.90 |    0.02 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    46.51 ns |   0.3297 ns |   0.3084 ns |  0.92 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    50.55 ns |   0.7593 ns |   0.7102 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    77.77 ns |   0.8393 ns |   0.7851 ns |  1.54 |    0.03 | 0.0559 |     - |     - |     176 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   128.46 ns |   1.6938 ns |   1.5844 ns |  0.91 |    0.02 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   127.35 ns |   1.3863 ns |   1.2289 ns |  0.90 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   141.55 ns |   1.6968 ns |   1.5042 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   129.65 ns |   1.5671 ns |   1.4659 ns |  0.92 |    0.01 | 0.0558 |     - |     - |     176 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,188.26 ns | 144.4650 ns | 135.1326 ns |  0.92 |    0.02 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 7,904.33 ns | 107.4945 ns | 100.5505 ns |  0.89 |    0.02 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 8,910.18 ns | 131.0285 ns | 122.5642 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 5,392.74 ns |  85.1604 ns |  79.6591 ns |  0.61 |    0.01 | 0.0534 |     - |     - |     176 B |
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
