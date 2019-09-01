using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    16.07 ns |  0.0781 ns |  0.0610 ns |  0.84 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             0 |    19.15 ns |  0.0697 ns |  0.0582 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             0 |   132.34 ns |  0.4594 ns |  0.4072 ns |  6.91 |    0.03 | 0.0482 |     - |     - |     152 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    21.61 ns |  0.2921 ns |  0.2439 ns |  0.80 |    0.01 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    26.86 ns |  0.1104 ns |  0.1032 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |   135.29 ns |  0.3407 ns |  0.2660 ns |  5.04 |    0.02 | 0.0482 |     - |     - |     152 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    75.75 ns |  0.1453 ns |  0.1359 ns |  0.77 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    98.91 ns |  0.2523 ns |  0.2107 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   220.74 ns |  1.3001 ns |  1.1525 ns |  2.23 |    0.01 | 0.0482 |     - |     - |     152 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,080.04 ns | 15.7449 ns | 13.9575 ns |  0.87 |    0.00 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 6,993.97 ns | 30.7214 ns | 23.9853 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 8,657.47 ns | 26.8425 ns | 23.7952 ns |  1.24 |    0.01 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaEnumerable_Aggreate : VanillaEnumerableBase
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
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(Numbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(Numbers, 0.0, (a, c) => a + c);
    }
}
