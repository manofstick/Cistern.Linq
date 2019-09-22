using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     77.74 ns |   0.6316 ns |   0.5908 ns |  0.98 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             0 |     94.27 ns |   0.5453 ns |   0.5101 ns |  1.18 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             0 |     79.61 ns |   0.5423 ns |   0.5072 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             0 |    120.75 ns |   0.9205 ns |   0.8610 ns |  1.52 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     92.90 ns |   0.5790 ns |   0.5416 ns |  0.99 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    108.16 ns |   0.5089 ns |   0.4511 ns |  1.15 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     94.33 ns |   0.3477 ns |   0.2904 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    126.02 ns |   1.0607 ns |   0.9921 ns |  1.34 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    226.80 ns |   0.5837 ns |   0.5175 ns |  0.91 |    0.01 | 0.0532 |     - |     - |     168 B |
    | CisternForLoop |            10 |    254.26 ns |   3.7079 ns |   3.4684 ns |  1.02 |    0.02 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    250.06 ns |   2.5906 ns |   2.1632 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    218.82 ns |   0.7313 ns |   0.6483 ns |  0.87 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,934.54 ns |  60.8827 ns |  56.9497 ns |  0.94 |    0.01 | 0.0458 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 14,990.32 ns |  26.1787 ns |  24.4876 ns |  1.01 |    0.01 | 0.0458 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 14,901.33 ns | 115.1537 ns | 107.7148 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     168 B |
    |    CisternLinq |          1000 |  9,093.50 ns |  62.4936 ns |  58.4565 ns |  0.61 |    0.01 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectEnumerable_Sum : WhereSelectEnumerableBase
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
