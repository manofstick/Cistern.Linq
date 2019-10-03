using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     47.01 ns |   0.8121 ns |   0.7596 ns |  0.93 |    0.02 | 0.0229 |     - |     - |      72 B |
    | CisternForLoop |             0 |     39.52 ns |   0.8114 ns |   0.7969 ns |  0.78 |    0.02 | 0.0229 |     - |     - |      72 B |
    |     SystemLinq |             0 |     50.63 ns |   0.9168 ns |   0.8576 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
    |    CisternLinq |             0 |     65.68 ns |   1.1699 ns |   1.0944 ns |  1.30 |    0.03 | 0.0229 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     57.32 ns |   1.0036 ns |   0.9387 ns |  0.96 |    0.02 | 0.0229 |     - |     - |      72 B |
    | CisternForLoop |             1 |     48.14 ns |   0.6974 ns |   0.6524 ns |  0.81 |    0.02 | 0.0229 |     - |     - |      72 B |
    |     SystemLinq |             1 |     59.62 ns |   1.0741 ns |   1.0047 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
    |    CisternLinq |             1 |     78.74 ns |   0.9249 ns |   0.8651 ns |  1.32 |    0.03 | 0.0229 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    153.32 ns |   1.8342 ns |   1.7157 ns |  0.93 |    0.01 | 0.0229 |     - |     - |      72 B |
    | CisternForLoop |            10 |    140.58 ns |   2.1639 ns |   2.0241 ns |  0.86 |    0.01 | 0.0229 |     - |     - |      72 B |
    |     SystemLinq |            10 |    164.12 ns |   1.5798 ns |   1.4777 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
    |    CisternLinq |            10 |    134.72 ns |   2.0701 ns |   1.9364 ns |  0.82 |    0.01 | 0.0229 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  8,309.17 ns | 116.7582 ns | 109.2157 ns |  0.75 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 |  8,323.22 ns | 130.0955 ns | 121.6914 ns |  0.75 |    0.02 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 11,045.24 ns | 120.5962 ns | 112.8057 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  6,175.45 ns |  60.1111 ns |  56.2280 ns |  0.56 |    0.01 | 0.0229 |     - |     - |      72 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Aggreate : SelectListBase
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
