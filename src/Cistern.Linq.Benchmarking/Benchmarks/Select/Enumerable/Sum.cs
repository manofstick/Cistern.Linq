using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     58.02 ns |   0.9456 ns |   0.8845 ns |  0.98 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             0 |     55.07 ns |   0.8702 ns |   0.8140 ns |  0.93 |    0.02 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             0 |     59.42 ns |   0.7272 ns |   0.6802 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             0 |     94.82 ns |   1.0176 ns |   0.9021 ns |  1.60 |    0.03 | 0.0635 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     70.46 ns |   1.1379 ns |   1.0644 ns |  0.96 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     70.38 ns |   0.8892 ns |   0.8317 ns |  0.96 |    0.02 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     73.06 ns |   1.0734 ns |   1.0040 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    102.68 ns |   1.4497 ns |   1.3561 ns |  1.41 |    0.03 | 0.0635 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    194.54 ns |   4.6653 ns |   3.8957 ns |  0.96 |    0.02 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    191.41 ns |   3.3168 ns |   2.9403 ns |  0.94 |    0.02 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    203.11 ns |   3.2124 ns |   3.0049 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    190.75 ns |   1.8104 ns |   1.6935 ns |  0.94 |    0.02 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 12,660.05 ns | 165.9565 ns | 155.2358 ns |  1.04 |    0.02 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,288.74 ns |  37.7659 ns |  31.5362 ns |  1.01 |    0.02 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 12,211.76 ns | 188.4240 ns | 176.2519 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  7,964.76 ns | 124.6100 ns | 116.5603 ns |  0.65 |    0.01 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Sum : SelectEnumerableBase
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
