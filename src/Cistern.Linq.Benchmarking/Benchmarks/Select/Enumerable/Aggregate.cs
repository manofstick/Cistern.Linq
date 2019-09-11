using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     65.04 ns |   0.6652 ns |   0.6222 ns |  0.94 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             0 |     64.03 ns |   0.5089 ns |   0.4760 ns |  0.92 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             0 |     69.48 ns |   0.6124 ns |   0.5728 ns |  1.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             0 |    102.18 ns |   0.4915 ns |   0.4598 ns |  1.47 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |             1 |     79.55 ns |   0.5703 ns |   0.5335 ns |  0.91 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     79.59 ns |   0.6007 ns |   0.5619 ns |  0.91 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     87.12 ns |   0.6819 ns |   0.6379 ns |  1.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    112.54 ns |   0.9107 ns |   0.8518 ns |  1.29 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |            10 |    220.53 ns |   1.5753 ns |   1.4735 ns |  0.93 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    215.54 ns |   2.0854 ns |   1.7414 ns |  0.91 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    237.92 ns |   2.0639 ns |   1.8296 ns |  1.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    223.82 ns |   1.6030 ns |   1.4994 ns |  0.94 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |          1000 | 14,187.09 ns | 101.7151 ns |  95.1444 ns |  0.91 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 13,907.22 ns |  68.0895 ns |  63.6910 ns |  0.89 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 15,591.12 ns | 148.9094 ns | 132.0043 ns |  1.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 11,287.82 ns |  89.0034 ns |  83.2539 ns |  0.72 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Aggreate : SelectEnumerableBase
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
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(SystemNumbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(CisternNumbers, 0.0, (a, c) => a + c);
    }
}
