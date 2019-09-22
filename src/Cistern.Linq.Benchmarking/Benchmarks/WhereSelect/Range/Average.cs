using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    100.54 ns |  0.5614 ns |  0.5251 ns |  0.98 |    0.01 | 0.0508 |     - |     - |     160 B |
    | CisternForLoop |             1 |     91.30 ns |  0.8268 ns |  0.7734 ns |  0.89 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |    102.98 ns |  0.4261 ns |  0.3986 ns |  1.00 |    0.00 | 0.0508 |     - |     - |     160 B |
    |    CisternLinq |             1 |    127.42 ns |  1.4489 ns |  1.3553 ns |  1.24 |    0.01 | 0.0608 |     - |     - |     192 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    249.38 ns |  2.1769 ns |  2.0362 ns |  1.01 |    0.01 | 0.0505 |     - |     - |     160 B |
    | CisternForLoop |            10 |    191.30 ns |  2.0404 ns |  1.8088 ns |  0.78 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    245.77 ns |  1.9672 ns |  1.8401 ns |  1.00 |    0.00 | 0.0505 |     - |     - |     160 B |
    |    CisternLinq |            10 |    163.19 ns |  1.0608 ns |  0.9923 ns |  0.66 |    0.01 | 0.0608 |     - |     - |     192 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,572.56 ns | 54.0801 ns | 47.9406 ns |  1.05 |    0.01 | 0.0458 |     - |     - |     160 B |
    | CisternForLoop |          1000 | 10,466.38 ns | 86.4604 ns | 80.8751 ns |  0.75 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 13,927.09 ns | 39.7998 ns | 37.2288 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     160 B |
    |    CisternLinq |          1000 |  4,842.52 ns | 34.7872 ns | 30.8379 ns |  0.35 |    0.00 | 0.0534 |     - |     - |     192 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRange_Average : WhereSelectRangeBase
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
