using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     78.24 ns |   0.8819 ns |   0.8249 ns |  0.97 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             0 |     93.65 ns |   0.5014 ns |   0.4187 ns |  1.15 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             0 |     81.16 ns |   0.5647 ns |   0.4715 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             0 |    132.75 ns |   1.3079 ns |   1.2234 ns |  1.64 |    0.02 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     92.38 ns |   0.5118 ns |   0.4274 ns |  0.96 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    107.95 ns |   0.3498 ns |   0.3272 ns |  1.12 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     96.58 ns |   0.5463 ns |   0.5110 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    144.40 ns |   1.4549 ns |   1.3609 ns |  1.50 |    0.02 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    226.81 ns |   0.5609 ns |   0.4972 ns |  0.89 |    0.01 | 0.0532 |     - |     - |     168 B |
    | CisternForLoop |            10 |    252.88 ns |   1.6888 ns |   1.5797 ns |  1.00 |    0.01 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    253.74 ns |   2.0457 ns |   1.8135 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    252.23 ns |   2.4836 ns |   2.2017 ns |  0.99 |    0.01 | 0.0682 |     - |     - |     216 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,912.44 ns |  37.6739 ns |  33.3970 ns |  0.86 |    0.01 | 0.0458 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 14,965.28 ns |  56.0266 ns |  49.6661 ns |  0.92 |    0.01 | 0.0458 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 16,184.61 ns | 131.2991 ns | 122.8172 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     168 B |
    |    CisternLinq |          1000 | 10,568.98 ns |  64.2083 ns |  60.0605 ns |  0.65 |    0.01 | 0.0610 |     - |     - |     216 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectEnumerable_Aggreate : WhereSelectEnumerableBase
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
