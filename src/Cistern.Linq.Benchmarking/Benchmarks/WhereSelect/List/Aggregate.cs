using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     64.85 ns |  0.7463 ns |  0.6980 ns |  0.99 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             0 |     77.04 ns |  0.5174 ns |  0.4840 ns |  1.17 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             0 |     65.65 ns |  0.6838 ns |  0.6397 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             0 |    112.84 ns |  0.2496 ns |  0.2212 ns |  1.72 |    0.02 | 0.0635 |     - |     - |     200 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     73.06 ns |  0.6665 ns |  0.5565 ns |  0.92 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             1 |     85.97 ns |  0.6191 ns |  0.5791 ns |  1.09 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     79.23 ns |  0.6822 ns |  0.6381 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    121.60 ns |  1.1322 ns |  1.0591 ns |  1.53 |    0.02 | 0.0634 |     - |     - |     200 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    178.44 ns |  1.1515 ns |  1.0771 ns |  0.91 |    0.01 | 0.0482 |     - |     - |     152 B |
    | CisternForLoop |            10 |    189.59 ns |  1.0117 ns |  0.9463 ns |  0.97 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    195.97 ns |  0.5979 ns |  0.4668 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    198.01 ns |  0.9044 ns |  0.7552 ns |  1.01 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,670.34 ns | 87.0748 ns | 72.7114 ns |  0.92 |    0.01 | 0.0458 |     - |     - |     152 B |
    | CisternForLoop |          1000 |  9,926.21 ns | 93.0032 ns | 86.9953 ns |  0.86 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 11,594.60 ns | 58.4375 ns | 54.6624 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  7,005.37 ns | 21.7134 ns | 19.2484 ns |  0.60 |    0.00 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectList_Aggreate : WhereSelectListBase
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
