using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |    36.39 ns |   0.6186 ns |   0.5786 ns |  0.98 |    0.02 |      - |     - |     - |         - |
    | CisternForLoop |             0 |    20.75 ns |   0.3339 ns |   0.3123 ns |  0.56 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    36.95 ns |   0.3998 ns |   0.3739 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    30.93 ns |   0.2928 ns |   0.2738 ns |  0.84 |    0.01 | 0.0101 |     - |     - |      32 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    55.87 ns |   0.6332 ns |   0.5614 ns |  0.95 |    0.02 | 0.0152 |     - |     - |      48 B |
    | CisternForLoop |             1 |    38.76 ns |   0.1981 ns |   0.1853 ns |  0.66 |    0.01 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    59.14 ns |   0.8768 ns |   0.8202 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    73.49 ns |   1.0091 ns |   0.9439 ns |  1.24 |    0.02 | 0.0483 |     - |     - |     152 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   143.44 ns |   1.7041 ns |   1.5940 ns |  0.94 |    0.02 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   113.78 ns |   1.8383 ns |   1.6296 ns |  0.75 |    0.02 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |            10 |   152.41 ns |   3.0703 ns |   2.8720 ns |  1.00 |    0.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |    97.89 ns |   0.9168 ns |   0.8576 ns |  0.64 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 7,746.84 ns |  88.4085 ns |  78.3718 ns |  1.01 |    0.02 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 | 7,304.35 ns | 106.4349 ns |  99.5593 ns |  0.95 |    0.01 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 7,659.13 ns | 124.6844 ns | 116.6299 ns |  1.00 |    0.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 2,206.56 ns |  33.2146 ns |  31.0689 ns |  0.29 |    0.01 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Sum : SelectArrayBase
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
