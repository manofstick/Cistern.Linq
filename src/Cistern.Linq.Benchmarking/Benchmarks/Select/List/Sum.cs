using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |    35.63 ns |   0.6805 ns |   0.6366 ns |  0.90 |    0.02 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             0 |    37.63 ns |   0.4841 ns |   0.4528 ns |  0.95 |    0.02 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             0 |    39.54 ns |   0.3794 ns |   0.3549 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             0 |    76.39 ns |   0.9454 ns |   0.8843 ns |  1.93 |    0.03 | 0.0533 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    46.10 ns |   0.7045 ns |   0.6245 ns |  0.93 |    0.02 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    46.69 ns |   0.4490 ns |   0.3980 ns |  0.95 |    0.02 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    49.33 ns |   0.7363 ns |   0.6888 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    79.36 ns |   0.9365 ns |   0.8301 ns |  1.61 |    0.03 | 0.0533 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   138.87 ns |   1.6571 ns |   1.5500 ns |  0.95 |    0.02 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   135.50 ns |   1.4593 ns |   1.3651 ns |  0.93 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   146.35 ns |   1.7779 ns |   1.6630 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   128.24 ns |   1.5687 ns |   1.4674 ns |  0.88 |    0.01 | 0.0532 |     - |     - |     168 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 8,881.70 ns |  76.6203 ns |  71.6707 ns |  1.01 |    0.02 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 8,500.98 ns | 128.8019 ns | 120.4813 ns |  0.97 |    0.02 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 8,756.29 ns | 124.8408 ns | 116.7762 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 4,382.79 ns |  61.7042 ns |  57.7182 ns |  0.50 |    0.01 | 0.0458 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Sum : SelectListBase
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
