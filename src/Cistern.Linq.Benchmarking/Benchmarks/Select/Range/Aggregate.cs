using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    32.50 ns |   0.1557 ns |   0.1456 ns |  1.21 |    0.01 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |    24.75 ns |   0.2168 ns |   0.2028 ns |  0.92 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    26.83 ns |   0.2675 ns |   0.2502 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    56.74 ns |   0.7397 ns |   0.6919 ns |  2.12 |    0.03 | 0.0076 |     - |     - |      24 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    68.00 ns |   0.5475 ns |   0.5122 ns |  1.10 |    0.02 | 0.0280 |     - |     - |      88 B |
    |  SystemForLoop |             1 |    60.31 ns |   1.2395 ns |   1.3263 ns |  0.98 |    0.03 | 0.0280 |     - |     - |      88 B |
    |     SystemLinq |             1 |    61.73 ns |   1.0014 ns |   0.9367 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |    94.64 ns |   0.9965 ns |   0.9322 ns |  1.53 |    0.03 | 0.0280 |     - |     - |      88 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   152.49 ns |   1.5639 ns |   1.4629 ns |  1.02 |    0.02 | 0.0279 |     - |     - |      88 B |
    |  SystemForLoop |            10 |   123.48 ns |   1.8174 ns |   1.7000 ns |  0.83 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |   148.97 ns |   1.5596 ns |   1.4588 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |   134.25 ns |   0.8498 ns |   0.7949 ns |  0.90 |    0.01 | 0.0279 |     - |     - |      88 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 8,921.44 ns | 141.3147 ns | 132.1858 ns |  1.05 |    0.02 | 0.0153 |     - |     - |      88 B |
    |  SystemForLoop |          1000 | 7,119.51 ns | 101.5500 ns |  94.9899 ns |  0.84 |    0.01 | 0.0229 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 8,504.39 ns | 117.2096 ns | 109.6379 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 | 4,429.10 ns |  84.1851 ns |  74.6279 ns |  0.52 |    0.01 | 0.0229 |     - |     - |      88 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Aggreate : SelectRangeBase
	{
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

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(SystemNumbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(CisternNumbers, 0.0, (a, c) => a + c);
    }
}
