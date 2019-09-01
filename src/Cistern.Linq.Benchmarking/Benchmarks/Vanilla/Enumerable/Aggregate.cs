using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    17.15 ns |  0.1338 ns |  0.1252 ns |  0.84 |    0.01 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             0 |    20.39 ns |  0.1382 ns |  0.1292 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             0 |    90.93 ns |  0.3764 ns |  0.3337 ns |  4.46 |    0.03 | 0.0380 |     - |     - |     120 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    22.87 ns |  0.1052 ns |  0.0984 ns |  0.80 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    28.43 ns |  0.1205 ns |  0.1068 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |    99.31 ns |  0.4292 ns |  0.4015 ns |  3.49 |    0.02 | 0.0380 |     - |     - |     120 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    82.07 ns |  0.5061 ns |  0.4734 ns |  0.78 |    0.01 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |   105.52 ns |  0.2907 ns |  0.2577 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   192.67 ns |  0.7954 ns |  0.7440 ns |  1.83 |    0.01 | 0.0379 |     - |     - |     120 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,462.03 ns | 40.8287 ns | 38.1912 ns |  0.87 |    0.01 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 7,441.80 ns | 23.3149 ns | 21.8087 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 9,154.10 ns | 45.3551 ns | 40.2061 ns |  1.23 |    0.01 | 0.0305 |     - |     - |     120 B |
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
