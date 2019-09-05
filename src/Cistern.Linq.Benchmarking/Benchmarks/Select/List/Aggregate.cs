using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     35.80 ns |   0.3914 ns |   0.3661 ns |  0.87 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             0 |     37.81 ns |   0.4544 ns |   0.4250 ns |  0.92 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             0 |     40.99 ns |   0.3699 ns |   0.3460 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             0 |     88.77 ns |   1.1161 ns |   1.0440 ns |  2.17 |    0.03 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     44.52 ns |   0.3938 ns |   0.3683 ns |  0.85 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |     46.79 ns |   0.5349 ns |   0.5004 ns |  0.90 |    0.02 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |     52.10 ns |   0.7613 ns |   0.7121 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |     98.32 ns |   1.0954 ns |   1.0246 ns |  1.89 |    0.03 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    136.86 ns |   1.3215 ns |   1.1715 ns |  0.88 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |    134.26 ns |   2.3120 ns |   2.1626 ns |  0.86 |    0.02 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |    155.98 ns |   1.5082 ns |   1.3370 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |    170.42 ns |   2.1205 ns |   1.9835 ns |  1.09 |    0.02 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  8,846.58 ns |  94.0999 ns |  88.0211 ns |  0.88 |    0.02 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 |  8,495.44 ns | 116.9273 ns | 109.3739 ns |  0.85 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 10,015.90 ns | 127.1981 ns | 118.9812 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  6,212.58 ns |  70.6718 ns |  66.1064 ns |  0.62 |    0.01 | 0.0534 |     - |     - |     184 B |
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
