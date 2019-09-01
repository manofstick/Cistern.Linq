using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    16.02 ns |  0.0491 ns |  0.0459 ns |  0.91 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             0 |    17.59 ns |  0.0581 ns |  0.0543 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             0 |   113.11 ns |  0.3019 ns |  0.2676 ns |  6.43 |    0.03 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    21.43 ns |  0.0707 ns |  0.0661 ns |  0.85 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    25.14 ns |  0.0968 ns |  0.0906 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |   123.83 ns |  0.3021 ns |  0.2522 ns |  4.93 |    0.02 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    75.81 ns |  0.1455 ns |  0.1361 ns |  0.92 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    82.12 ns |  0.1463 ns |  0.1369 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   168.34 ns |  0.4025 ns |  0.3765 ns |  2.05 |    0.01 | 0.0432 |     - |     - |     136 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,081.63 ns | 14.4323 ns | 13.5000 ns |  0.95 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 6,416.29 ns | 16.2187 ns | 15.1709 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 5,596.73 ns | 15.6561 ns | 14.6447 ns |  0.87 |    0.00 | 0.0381 |     - |     - |     136 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaEnumerable_Sum : VanillaEnumerableBase
    {
		[Benchmark]
		public double ForLoop()
		{
			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
			}
			return sum;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Sum(Numbers);
	
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Sum(Numbers);
	}
}
