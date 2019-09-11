using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |    40.76 ns |  0.2156 ns |  0.2016 ns |  0.99 |      - |     - |     - |         - |
    | CisternForLoop |             0 |    23.60 ns |  0.2059 ns |  0.1926 ns |  0.58 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    41.03 ns |  0.2700 ns |  0.2525 ns |  1.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    35.15 ns |  0.2000 ns |  0.1773 ns |  0.86 | 0.0101 |     - |     - |      32 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |             1 |    62.73 ns |  0.7485 ns |  0.6636 ns |  0.94 | 0.0151 |     - |     - |      48 B |
    | CisternForLoop |             1 |    43.42 ns |  0.2085 ns |  0.1951 ns |  0.65 | 0.0178 |     - |     - |      56 B |
    |     SystemLinq |             1 |    66.88 ns |  0.2991 ns |  0.2497 ns |  1.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    59.85 ns |  0.4790 ns |  0.4246 ns |  0.89 | 0.0279 |     - |     - |      88 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |            10 |   160.55 ns |  0.9630 ns |  0.9008 ns |  0.93 | 0.0150 |     - |     - |      48 B |
    | CisternForLoop |            10 |   129.58 ns |  1.2419 ns |  1.1617 ns |  0.75 | 0.0176 |     - |     - |      56 B |
    |     SystemLinq |            10 |   172.29 ns |  1.3361 ns |  1.1844 ns |  1.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |    87.11 ns |  1.1278 ns |  0.9418 ns |  0.51 | 0.0279 |     - |     - |      88 B |
    |                |               |             |            |            |       |        |       |       |           |
    |  SystemForLoop |          1000 | 8,618.20 ns | 74.6109 ns | 62.3035 ns |  1.01 |      - |     - |     - |      48 B |
    | CisternForLoop |          1000 | 8,142.79 ns | 56.9005 ns | 53.2248 ns |  0.95 | 0.0153 |     - |     - |      56 B |
    |     SystemLinq |          1000 | 8,556.67 ns | 71.8850 ns | 67.2413 ns |  1.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 2,461.79 ns | 12.0927 ns | 10.7199 ns |  0.29 | 0.0267 |     - |     - |      88 B |
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
