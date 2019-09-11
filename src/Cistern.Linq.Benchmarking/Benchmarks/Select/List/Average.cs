using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     51.69 ns |  0.5522 ns |  0.5165 ns |  0.93 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |     57.23 ns |  0.4303 ns |  0.4025 ns |  1.03 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |     55.58 ns |  0.2866 ns |  0.2681 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |     96.53 ns |  0.8545 ns |  0.7993 ns |  1.74 |    0.01 | 0.0533 |     - |     - |     168 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    148.21 ns |  1.1199 ns |  1.0475 ns |  0.94 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |    160.44 ns |  0.9018 ns |  0.7994 ns |  1.02 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |    157.91 ns |  1.3686 ns |  1.2802 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |    156.48 ns |  1.3270 ns |  1.1764 ns |  0.99 |    0.01 | 0.0532 |     - |     - |     168 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  9,859.65 ns | 66.3881 ns | 62.0994 ns |  1.00 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 10,204.39 ns | 98.6463 ns | 92.2738 ns |  1.03 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 |  9,860.23 ns | 89.4316 ns | 83.6544 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  5,877.19 ns | 82.3815 ns | 77.0597 ns |  0.60 |    0.01 | 0.0458 |     - |     - |     168 B |
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
