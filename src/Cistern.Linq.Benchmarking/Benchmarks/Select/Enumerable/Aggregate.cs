using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     72.47 ns |   1.4538 ns |   1.7307 ns |  1.01 |    0.03 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             0 |     61.22 ns |   1.2127 ns |   1.1910 ns |  0.85 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             0 |     71.77 ns |   1.0442 ns |   0.9768 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             0 |     97.31 ns |   2.0733 ns |   2.4681 ns |  1.36 |    0.05 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.03 ns |   0.9073 ns |   0.8487 ns |  0.95 |    0.02 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             1 |     73.06 ns |   0.9085 ns |   0.8498 ns |  0.86 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             1 |     85.00 ns |   0.9510 ns |   0.8895 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             1 |    114.27 ns |   1.0229 ns |   0.9568 ns |  1.34 |    0.02 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    193.30 ns |   1.9843 ns |   1.8561 ns |  0.89 |    0.01 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |            10 |    189.16 ns |   2.6835 ns |   2.5101 ns |  0.88 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |            10 |    216.16 ns |   1.6781 ns |   1.5697 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |            10 |    199.02 ns |   2.6220 ns |   2.3243 ns |  0.92 |    0.01 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 11,436.56 ns | 110.8796 ns | 103.7168 ns |  0.88 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 11,732.29 ns | 201.3866 ns | 188.3771 ns |  0.91 |    0.02 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 12,926.30 ns | 159.6441 ns | 141.5203 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,657.20 ns | 125.9797 ns | 117.8414 ns |  0.75 |    0.01 | 0.0305 |     - |     - |     104 B |
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
