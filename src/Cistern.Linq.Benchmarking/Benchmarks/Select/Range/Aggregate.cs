using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    30.27 ns |   0.4145 ns |   0.3878 ns |  1.13 |    0.02 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |    25.43 ns |   0.1863 ns |   0.1742 ns |  0.95 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    26.70 ns |   0.2080 ns |   0.1737 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    61.04 ns |   1.2132 ns |   1.6606 ns |  2.26 |    0.08 | 0.0076 |     - |     - |      24 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    63.76 ns |   0.7338 ns |   0.6864 ns |  0.94 |    0.01 | 0.0254 |     - |     - |      80 B |
    |  SystemForLoop |             1 |    61.52 ns |   0.7619 ns |   0.6754 ns |  0.91 |    0.01 | 0.0280 |     - |     - |      88 B |
    |     SystemLinq |             1 |    67.50 ns |   0.8192 ns |   0.7663 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |    93.25 ns |   1.1743 ns |   1.0984 ns |  1.38 |    0.03 | 0.0254 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   150.97 ns |   1.5355 ns |   1.4363 ns |  1.00 |    0.01 | 0.0253 |     - |     - |      80 B |
    |  SystemForLoop |            10 |   129.38 ns |   1.7375 ns |   1.6253 ns |  0.85 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |   151.53 ns |   1.8209 ns |   1.7032 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |   138.22 ns |   1.7530 ns |   1.6398 ns |  0.91 |    0.02 | 0.0253 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 9,497.30 ns | 200.4194 ns | 351.0185 ns |  1.10 |    0.05 | 0.0153 |     - |     - |      80 B |
    |  SystemForLoop |          1000 | 7,550.21 ns |  82.6979 ns |  77.3557 ns |  0.85 |    0.01 | 0.0229 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 8,913.59 ns |  76.6116 ns |  71.6625 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 | 4,880.11 ns |  95.8054 ns | 117.6576 ns |  0.55 |    0.02 | 0.0229 |     - |     - |      80 B |
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
