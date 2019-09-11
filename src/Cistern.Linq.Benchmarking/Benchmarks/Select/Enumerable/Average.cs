using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.32 ns |   0.6865 ns |   0.5733 ns |  0.99 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     77.65 ns |   0.5917 ns |   0.5534 ns |  0.95 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     82.10 ns |   0.7140 ns |   0.6679 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    132.39 ns |   1.0555 ns |   0.9357 ns |  1.61 |    0.02 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    224.99 ns |   1.1030 ns |   1.0318 ns |  1.01 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    222.54 ns |   0.9998 ns |   0.8863 ns |  1.00 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    222.96 ns |   1.5971 ns |   1.4939 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    229.85 ns |   1.3566 ns |   1.2689 ns |  1.03 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,661.54 ns |  92.4339 ns |  86.4627 ns |  1.08 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 14,318.29 ns | 165.3883 ns | 154.7043 ns |  1.05 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,618.77 ns |  87.3042 ns |  81.6644 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 10,699.20 ns |  54.0936 ns |  50.5992 ns |  0.79 |    0.00 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Average : SelectEnumerableBase
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
