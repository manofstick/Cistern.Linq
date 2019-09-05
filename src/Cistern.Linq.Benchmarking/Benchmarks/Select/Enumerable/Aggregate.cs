using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     58.82 ns |   0.4983 ns |   0.4661 ns |  0.94 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             0 |     55.63 ns |   0.7943 ns |   0.7430 ns |  0.89 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             0 |     62.74 ns |   0.2477 ns |   0.2317 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             0 |    105.41 ns |   1.5396 ns |   1.4401 ns |  1.68 |    0.02 | 0.0685 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     70.82 ns |   0.6732 ns |   0.5968 ns |  0.92 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     69.93 ns |   1.1763 ns |   1.1003 ns |  0.90 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     77.15 ns |   0.9111 ns |   0.8077 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    119.68 ns |   1.5618 ns |   1.4609 ns |  1.55 |    0.03 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    196.23 ns |   2.1731 ns |   2.0327 ns |  0.94 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    192.48 ns |   2.0576 ns |   1.9247 ns |  0.93 |    0.02 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    207.88 ns |   2.3403 ns |   2.1891 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    220.52 ns |   4.1732 ns |   3.9036 ns |  1.06 |    0.03 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,654.88 ns | 141.4881 ns | 132.3480 ns |  0.92 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,351.32 ns | 145.2455 ns | 135.8627 ns |  0.89 |    0.02 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,832.10 ns | 209.0870 ns | 195.5801 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,941.32 ns | 101.0971 ns |  94.5663 ns |  0.72 |    0.01 | 0.0610 |     - |     - |     216 B |
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
