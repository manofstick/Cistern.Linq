using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    24.52 ns |   0.3641 ns |   0.3406 ns |  0.94 |    0.02 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             0 |    26.05 ns |   0.5478 ns |   0.5626 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             0 |    82.12 ns |   1.6703 ns |   2.7906 ns |  3.23 |    0.15 | 0.0153 |     - |     - |      48 B |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |             1 |    29.05 ns |   0.4407 ns |   0.3907 ns |  0.86 |    0.02 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             1 |    33.65 ns |   0.4949 ns |   0.4629 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             1 |    91.20 ns |   1.6290 ns |   1.5237 ns |  2.71 |    0.06 | 0.0153 |     - |     - |      48 B |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |            10 |    74.58 ns |   1.0766 ns |   1.0071 ns |  0.74 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |            10 |   100.26 ns |   0.8685 ns |   0.8124 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |            10 |   159.24 ns |   1.8603 ns |   1.7401 ns |  1.59 |    0.02 | 0.0153 |     - |     - |      48 B |
    |             |               |             |             |             |       |         |        |       |       |           |
    |     ForLoop |          1000 | 5,144.31 ns |  89.7844 ns |  74.9741 ns |  0.69 |    0.02 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 7,405.92 ns | 142.7008 ns | 140.1512 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |          1000 | 6,715.17 ns |  66.3556 ns |  55.4099 ns |  0.91 |    0.02 | 0.0153 |     - |     - |      48 B |
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
