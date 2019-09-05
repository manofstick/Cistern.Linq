using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    20.80 ns |   0.2278 ns |   0.2131 ns |  0.53 |    0.01 |      - |     - |     - |         - |
    |  SystemForLoop |             0 |    36.48 ns |   0.4974 ns |   0.4653 ns |  0.92 |    0.02 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    39.61 ns |   0.6095 ns |   0.5701 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    39.66 ns |   0.6109 ns |   0.5714 ns |  1.00 |    0.02 | 0.0152 |     - |     - |      48 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    38.37 ns |   0.6709 ns |   0.6276 ns |  0.63 |    0.01 | 0.0178 |     - |     - |      56 B |
    |  SystemForLoop |             1 |    54.46 ns |   0.7328 ns |   0.6855 ns |  0.89 |    0.02 | 0.0152 |     - |     - |      48 B |
    |     SystemLinq |             1 |    60.78 ns |   0.6366 ns |   0.5644 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    86.33 ns |   1.2743 ns |   1.1919 ns |  1.42 |    0.03 | 0.0533 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   114.51 ns |   1.7306 ns |   1.6188 ns |  0.75 |    0.01 | 0.0178 |     - |     - |      56 B |
    |  SystemForLoop |            10 |   141.82 ns |   1.7569 ns |   1.6434 ns |  0.93 |    0.02 | 0.0150 |     - |     - |      48 B |
    |     SystemLinq |            10 |   153.21 ns |   1.7707 ns |   1.4786 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |   125.30 ns |   1.8214 ns |   1.7037 ns |  0.82 |    0.02 | 0.0532 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 7,313.49 ns | 109.8505 ns |  97.3796 ns |  0.82 |    0.01 | 0.0153 |     - |     - |      56 B |
    |  SystemForLoop |          1000 | 7,610.35 ns | 118.6026 ns | 110.9410 ns |  0.86 |    0.02 | 0.0076 |     - |     - |      48 B |
    |     SystemLinq |          1000 | 8,877.22 ns |  89.9063 ns |  84.0984 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 3,711.91 ns |  58.9129 ns |  55.1071 ns |  0.42 |    0.01 | 0.0496 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Aggreate : SelectArrayBase
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
