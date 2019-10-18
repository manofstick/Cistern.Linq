using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2988 ns |  0.0100 ns |  0.0089 ns |  0.03 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             0 |     9.9577 ns |  0.0281 ns |  0.0249 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    38.2273 ns |  0.1549 ns |  0.1449 ns |  3.84 |    0.02 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     0.3486 ns |  0.0053 ns |  0.0050 ns |  0.02 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    22.0511 ns |  0.1539 ns |  0.1440 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    48.0844 ns |  0.9917 ns |  1.2180 ns |  2.21 |    0.04 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     4.1193 ns |  0.0185 ns |  0.0173 ns |  0.04 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    95.5541 ns |  1.1075 ns |  0.9248 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    74.1279 ns |  0.3405 ns |  0.3185 ns |  0.78 |    0.01 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,249.6033 ns |  3.2185 ns |  3.0106 ns |  0.18 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 6,850.5617 ns | 35.3652 ns | 31.3503 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 2,988.7938 ns |  9.2939 ns |  8.2388 ns |  0.44 |    0.00 |      - |     - |     - |         - |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Aggreate : VanillaArrayBase
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
