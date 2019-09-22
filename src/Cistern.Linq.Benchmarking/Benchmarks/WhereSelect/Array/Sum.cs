using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |    36.81 ns |  0.2790 ns |  0.2609 ns |  0.99 |      - |     - |     - |         - |
    | CisternForLoop |             0 |    41.44 ns |  0.2617 ns |  0.2185 ns |  1.11 | 0.0076 |     - |     - |      24 B |
    |     SystemLinq |             0 |    37.30 ns |  0.2574 ns |  0.2408 ns |  1.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    53.96 ns |  0.2476 ns |  0.2195 ns |  1.45 | 0.0178 |     - |     - |      56 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |             1 |    65.39 ns |  0.5879 ns |  0.5500 ns |  0.87 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |    76.95 ns |  0.6636 ns |  0.6208 ns |  1.03 | 0.0380 |     - |     - |     120 B |
    |     SystemLinq |             1 |    74.91 ns |  0.5431 ns |  0.5081 ns |  1.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    90.13 ns |  0.4695 ns |  0.3921 ns |  1.20 | 0.0483 |     - |     - |     152 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |            10 |   159.60 ns |  1.1751 ns |  1.0992 ns |  0.95 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |   163.43 ns |  1.0097 ns |  0.8950 ns |  0.97 | 0.0379 |     - |     - |     120 B |
    |     SystemLinq |            10 |   168.14 ns |  1.1912 ns |  1.1143 ns |  1.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |   125.12 ns |  1.2526 ns |  0.9780 ns |  0.74 | 0.0482 |     - |     - |     152 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |          1000 | 8,929.01 ns | 86.3304 ns | 80.7535 ns |  0.99 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 8,364.99 ns | 68.0817 ns | 63.6836 ns |  0.93 | 0.0305 |     - |     - |     120 B |
    |     SystemLinq |          1000 | 8,980.42 ns | 94.6309 ns | 83.8878 ns |  1.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 3,635.45 ns |  5.3764 ns |  4.7660 ns |  0.40 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectArray_Sum : WhereSelectArrayBase
    {
        [Benchmark]
        public double SystemForLoop()
        {
            double sum = 0;
            foreach (var n in SystemNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark]
        public double CisternForLoop()
        {
            double sum = 0;
            foreach (var n in CisternNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Sum(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Sum(CisternNumbers);
	}
}
