using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.List
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     65.13 ns |   1.0741 ns |   1.0047 ns |  1.00 |    0.02 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             0 |     77.44 ns |   0.6081 ns |   0.5390 ns |  1.19 |    0.02 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             0 |     64.91 ns |   0.7264 ns |   0.6795 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             0 |    103.25 ns |   0.5170 ns |   0.4583 ns |  1.59 |    0.02 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     73.13 ns |   0.7391 ns |   0.6914 ns |  0.95 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             1 |     86.04 ns |   0.5276 ns |   0.4936 ns |  1.12 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     77.13 ns |   0.5087 ns |   0.4759 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    105.67 ns |   0.3831 ns |   0.3396 ns |  1.37 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    179.00 ns |   0.7911 ns |   0.7400 ns |  0.91 |    0.01 | 0.0482 |     - |     - |     152 B |
    | CisternForLoop |            10 |    187.51 ns |   0.8356 ns |   0.7816 ns |  0.95 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    197.38 ns |   1.1174 ns |   1.0452 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    159.86 ns |   2.1952 ns |   1.9460 ns |  0.81 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,658.58 ns |  61.2804 ns |  57.3217 ns |  0.89 |    0.01 | 0.0458 |     - |     - |     152 B |
    | CisternForLoop |          1000 |  9,926.35 ns | 122.0496 ns | 114.1652 ns |  0.83 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 12,005.98 ns |  61.4047 ns |  57.4380 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  5,448.03 ns |  37.9272 ns |  33.6214 ns |  0.45 |    0.00 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectList_Sum : WhereSelectListBase
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
		public double CisternLinq()  => Cistern.Linq.Enumerable.Sum(CisternNumbers);
	}
}
