using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    61.89 ns |  0.6590 ns |  0.6164 ns |  0.98 |    0.01 | 0.0151 |     - |     - |      48 B |
    | CisternForLoop |             1 |    44.52 ns |  0.3699 ns |  0.3460 ns |  0.70 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    63.23 ns |  0.4136 ns |  0.3869 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    72.87 ns |  0.5153 ns |  0.4820 ns |  1.15 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   147.69 ns |  1.2035 ns |  1.1258 ns |  1.06 |    0.01 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   128.51 ns |  0.7533 ns |  0.6678 ns |  0.92 |    0.01 | 0.0176 |     - |     - |      56 B |
    |     SystemLinq |            10 |   139.51 ns |  0.9907 ns |  0.9267 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |   126.10 ns |  0.6192 ns |  0.5792 ns |  0.90 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,207.42 ns | 68.5853 ns | 64.1547 ns |  0.96 |    0.01 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 | 7,511.66 ns | 67.6145 ns | 63.2466 ns |  0.88 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 8,547.71 ns | 80.9228 ns | 75.6952 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 5,231.96 ns | 35.3337 ns | 33.0512 ns |  0.61 |    0.01 | 0.0458 |     - |     - |     152 B |
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
