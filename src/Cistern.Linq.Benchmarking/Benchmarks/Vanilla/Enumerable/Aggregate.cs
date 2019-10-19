using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    25.95 ns |  0.5586 ns |  0.4665 ns |  0.97 |    0.02 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             0 |    26.79 ns |  0.2912 ns |  0.2724 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             0 |    83.54 ns |  0.9390 ns |  0.8784 ns |  3.12 |    0.04 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    29.66 ns |  0.3327 ns |  0.3112 ns |  0.82 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             1 |    36.33 ns |  0.3832 ns |  0.3584 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             1 |   100.10 ns |  1.1625 ns |  1.0874 ns |  2.76 |    0.04 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    81.40 ns |  0.8664 ns |  0.8105 ns |  0.79 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |            10 |   103.03 ns |  0.8664 ns |  0.8104 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |            10 |   166.74 ns |  2.0108 ns |  1.8809 ns |  1.62 |    0.03 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 5,839.48 ns | 64.3005 ns | 60.1467 ns |  0.87 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 6,729.27 ns | 47.6915 ns | 44.6106 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |          1000 | 6,814.05 ns | 49.3289 ns | 43.7288 ns |  1.01 |    0.01 | 0.0153 |     - |     - |      48 B |
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
