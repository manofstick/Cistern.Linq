using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |    40.23 ns |  0.3029 ns |  0.2834 ns |  0.91 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             0 |    42.30 ns |  0.2596 ns |  0.2428 ns |  0.96 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             0 |    44.28 ns |  0.2540 ns |  0.2252 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             0 |    68.66 ns |  0.6275 ns |  0.5869 ns |  1.55 |    0.02 | 0.0330 |     - |     - |     104 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |    51.36 ns |  0.4311 ns |  0.4032 ns |  0.91 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |    54.41 ns |  0.4099 ns |  0.3834 ns |  0.96 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |    56.59 ns |  0.4120 ns |  0.3853 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |    72.85 ns |  0.3511 ns |  0.3284 ns |  1.29 |    0.01 | 0.0330 |     - |     - |     104 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |   147.41 ns |  0.8232 ns |  0.7700 ns |  0.92 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |   151.36 ns |  0.9092 ns |  0.8504 ns |  0.95 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |   159.52 ns |  1.6235 ns |  1.4392 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |   112.93 ns |  1.0777 ns |  0.9553 ns |  0.71 |    0.01 | 0.0330 |     - |     - |     104 B |
    |                |               |             |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 9,506.18 ns | 82.7180 ns | 77.3745 ns |  0.98 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 | 9,827.71 ns | 87.5087 ns | 77.5742 ns |  1.01 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 9,740.83 ns | 71.5066 ns | 66.8873 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 | 3,738.06 ns | 19.5371 ns | 18.2750 ns |  0.38 |    0.00 | 0.0305 |     - |     - |     104 B |
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
