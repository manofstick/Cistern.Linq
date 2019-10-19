using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     3.433 ns |  0.0381 ns |  0.0357 ns |  0.18 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    19.459 ns |  0.2850 ns |  0.2666 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    34.204 ns |  0.5027 ns |  0.4702 ns |  1.76 |    0.03 |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     6.116 ns |  0.1356 ns |  0.1269 ns |  0.22 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    28.279 ns |  0.3863 ns |  0.3614 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    48.451 ns |  0.5584 ns |  0.5223 ns |  1.71 |    0.03 |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    27.991 ns |  0.5494 ns |  0.4588 ns |  0.22 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |   126.778 ns |  1.4783 ns |  1.3828 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |    97.139 ns |  1.7520 ns |  1.6389 ns |  0.77 |    0.01 |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 2,416.089 ns | 34.9173 ns | 32.6617 ns |  0.26 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 9,276.823 ns | 90.6049 ns | 84.7519 ns |  1.00 |    0.00 |      - |     - |     - |      40 B |
    | CisternLinq |          1000 | 3,630.037 ns | 68.9195 ns | 67.6881 ns |  0.39 |    0.01 |      - |     - |     - |         - |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaList_Aggreate : VanillaListBase
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
