using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    26.81 ns |  0.2121 ns |  0.1984 ns |  0.95 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             0 |    28.32 ns |  0.2554 ns |  0.2133 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             0 |    95.80 ns |  1.0179 ns |  0.9522 ns |  3.37 |    0.04 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    32.19 ns |  0.1682 ns |  0.1573 ns |  0.91 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |             1 |    35.52 ns |  0.1407 ns |  0.1316 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |             1 |    97.97 ns |  0.2742 ns |  0.2431 ns |  2.76 |    0.01 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    80.83 ns |  0.2235 ns |  0.2090 ns |  0.74 |    0.00 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |            10 |   108.99 ns |  0.2525 ns |  0.2362 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |            10 |   180.85 ns |  0.3633 ns |  0.3221 ns |  1.66 |    0.01 | 0.0153 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,168.75 ns | 23.4650 ns | 21.9492 ns |  0.86 |    0.00 | 0.0153 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 7,145.95 ns | 11.9239 ns | 10.5702 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    | CisternLinq |          1000 | 7,224.29 ns | 25.2416 ns | 21.0779 ns |  1.01 |    0.00 | 0.0153 |     - |     - |      48 B |
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
