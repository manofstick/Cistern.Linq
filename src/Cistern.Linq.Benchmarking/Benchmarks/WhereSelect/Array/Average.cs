using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |          NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |             |             |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    66.95 ns |   0.5778 ns |  0.5405 ns |  0.96 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |    77.52 ns |   0.4758 ns |  0.4451 ns |  1.11 |    0.01 | 0.0380 |     - |     - |     120 B |
    |     SystemLinq |             1 |    70.06 ns |   0.5631 ns |  0.5268 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    99.73 ns |   0.5809 ns |  0.4536 ns |  1.42 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |             |             |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   157.50 ns |   1.2113 ns |  1.1330 ns |  0.94 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |   172.34 ns |   1.2125 ns |  1.1341 ns |  1.03 |    0.01 | 0.0379 |     - |     - |     120 B |
    |     SystemLinq |            10 |   167.25 ns |   1.1009 ns |  1.0298 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |   131.44 ns |   1.0950 ns |  1.0243 ns |  0.79 |    0.01 | 0.0508 |     - |     - |     160 B |
    |                |               |             |             |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,413.06 ns | 101.2620 ns | 94.7205 ns |  0.95 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 9,820.29 ns |  64.7357 ns | 60.5538 ns |  1.11 |    0.01 | 0.0305 |     - |     - |     120 B |
    |     SystemLinq |          1000 | 8,834.28 ns |  74.6221 ns | 69.8016 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 3,962.83 ns |  31.0222 ns | 29.0182 ns |  0.45 |    0.01 | 0.0458 |     - |     - |     160 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectArray_Average : WhereSelectArrayBase
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
