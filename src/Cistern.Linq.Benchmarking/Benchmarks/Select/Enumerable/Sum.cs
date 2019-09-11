using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     65.24 ns |   0.6068 ns |   0.5676 ns |  0.98 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             0 |     64.06 ns |   0.4612 ns |   0.4314 ns |  0.96 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             0 |     66.40 ns |   0.5362 ns |   0.5015 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             0 |     89.43 ns |   0.6514 ns |   0.6093 ns |  1.35 |    0.02 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     79.74 ns |   0.6429 ns |   0.6014 ns |  0.98 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     79.46 ns |   0.8984 ns |   0.7964 ns |  0.97 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     81.72 ns |   0.6286 ns |   0.5880 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     96.51 ns |   1.0389 ns |   0.9209 ns |  1.18 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    221.02 ns |   1.6999 ns |   1.5901 ns |  0.96 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    217.64 ns |   1.8396 ns |   1.5362 ns |  0.95 |    0.00 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    229.88 ns |   1.7033 ns |   1.5933 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    184.90 ns |   1.0517 ns |   0.9323 ns |  0.80 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,256.29 ns |  82.4765 ns |  77.1485 ns |  1.04 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 13,991.83 ns | 119.3702 ns | 111.6589 ns |  1.03 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,646.98 ns |  81.3284 ns |  76.0747 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  7,915.01 ns |  62.5067 ns |  58.4688 ns |  0.58 |    0.01 | 0.0305 |     - |     - |     136 B |
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
