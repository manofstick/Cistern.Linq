using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     72.79 ns |  0.4997 ns |  0.4430 ns |  0.94 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             1 |     85.93 ns |  0.6558 ns |  0.5813 ns |  1.11 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     77.45 ns |  0.6886 ns |  0.6441 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    106.04 ns |  0.4082 ns |  0.3819 ns |  1.37 |    0.01 | 0.0609 |     - |     - |     192 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    178.18 ns |  1.1390 ns |  1.0654 ns |  0.92 |    0.01 | 0.0482 |     - |     - |     152 B |
    | CisternForLoop |            10 |    191.87 ns |  0.8429 ns |  0.7884 ns |  1.00 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    192.81 ns |  0.9341 ns |  0.8738 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    156.78 ns |  1.3122 ns |  1.1633 ns |  0.81 |    0.01 | 0.0608 |     - |     - |     192 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,401.72 ns | 62.8014 ns | 58.7444 ns |  0.95 |    0.01 | 0.0458 |     - |     - |     152 B |
    | CisternForLoop |          1000 | 10,744.77 ns | 55.4357 ns | 49.1423 ns |  0.98 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 10,931.64 ns | 65.8290 ns | 54.9702 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  5,197.00 ns | 29.8663 ns | 27.9369 ns |  0.48 |    0.00 | 0.0534 |     - |     - |     192 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectList_Average : WhereSelectListBase
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
