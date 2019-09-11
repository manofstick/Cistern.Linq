using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    23.59 ns |   0.1863 ns |   0.1555 ns |  0.53 |      - |     - |     - |         - |
    |  SystemForLoop |             0 |    40.98 ns |   0.2108 ns |   0.1972 ns |  0.93 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    44.24 ns |   0.3074 ns |   0.2875 ns |  1.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    46.72 ns |   0.2811 ns |   0.2629 ns |  1.06 | 0.0152 |     - |     - |      48 B |
    |                |               |             |             |             |       |        |       |       |           |
    | CisternForLoop |             1 |    43.53 ns |   0.2654 ns |   0.2353 ns |  0.64 | 0.0178 |     - |     - |      56 B |
    |  SystemForLoop |             1 |    62.56 ns |   0.3642 ns |   0.3406 ns |  0.92 | 0.0151 |     - |     - |      48 B |
    |     SystemLinq |             1 |    67.73 ns |   0.5346 ns |   0.5000 ns |  1.00 | 0.0151 |     - |     - |      48 B |
    |    CisternLinq |             1 |    74.47 ns |   0.5337 ns |   0.4993 ns |  1.10 | 0.0330 |     - |     - |     104 B |
    |                |               |             |             |             |       |        |       |       |           |
    | CisternForLoop |            10 |   129.83 ns |   1.0843 ns |   1.0142 ns |  0.75 | 0.0176 |     - |     - |      56 B |
    |  SystemForLoop |            10 |   160.76 ns |   1.1633 ns |   1.0312 ns |  0.93 | 0.0150 |     - |     - |      48 B |
    |     SystemLinq |            10 |   172.99 ns |   1.1336 ns |   1.0603 ns |  1.00 | 0.0150 |     - |     - |      48 B |
    |    CisternLinq |            10 |   128.86 ns |   1.6246 ns |   1.2683 ns |  0.74 | 0.0329 |     - |     - |     104 B |
    |                |               |             |             |             |       |        |       |       |           |
    | CisternForLoop |          1000 | 8,182.00 ns |  52.6021 ns |  49.2040 ns |  0.83 | 0.0153 |     - |     - |      56 B |
    |  SystemForLoop |          1000 | 8,569.24 ns |  59.1201 ns |  55.3009 ns |  0.87 |      - |     - |     - |      48 B |
    |     SystemLinq |          1000 | 9,844.71 ns | 122.5884 ns | 114.6692 ns |  1.00 |      - |     - |     - |      48 B |
    |    CisternLinq |          1000 | 5,274.09 ns |  41.1239 ns |  38.4673 ns |  0.54 | 0.0305 |     - |     - |     104 B |
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
