using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    18.31 ns |   0.1907 ns |  0.1593 ns |  0.57 |    0.01 |      - |     - |     - |         - |
    |  SystemForLoop |             0 |    30.47 ns |   0.5077 ns |  0.4749 ns |  0.95 |    0.02 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    32.15 ns |   0.4552 ns |  0.4258 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    49.22 ns |   0.6112 ns |  0.5717 ns |  1.53 |    0.02 |      - |     - |     - |         - |
    |                |               |             |             |            |       |         |        |       |       |           |
    | CisternForLoop |             1 |    34.95 ns |   0.3548 ns |  0.3319 ns |  0.55 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |             1 |    56.76 ns |   0.9690 ns |  0.9064 ns |  0.90 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |             1 |    63.35 ns |   0.8518 ns |  0.7968 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |             1 |    71.54 ns |   1.0517 ns |  0.9838 ns |  1.13 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |            |       |         |        |       |       |           |
    | CisternForLoop |            10 |   107.45 ns |   1.5900 ns |  1.4095 ns |  0.75 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |            10 |   127.61 ns |   1.9467 ns |  1.8210 ns |  0.89 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |            10 |   142.74 ns |   2.3139 ns |  2.1644 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |            10 |   108.18 ns |   1.7759 ns |  1.6611 ns |  0.76 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |            |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 6,360.49 ns |  76.6937 ns | 67.9870 ns |  0.72 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |          1000 | 6,419.12 ns |  85.7469 ns | 76.0124 ns |  0.72 |    0.01 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |          1000 | 8,855.78 ns | 104.8780 ns | 87.5778 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |          1000 | 3,679.02 ns |  33.7785 ns | 31.5965 ns |  0.42 |    0.00 | 0.0153 |     - |     - |      48 B |
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
