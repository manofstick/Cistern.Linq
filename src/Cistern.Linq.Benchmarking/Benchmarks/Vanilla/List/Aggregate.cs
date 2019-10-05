using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     3.848 ns |   0.0493 ns |  0.0461 ns |  0.20 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    18.987 ns |   0.2720 ns |  0.2544 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             0 |    31.702 ns |   0.4127 ns |  0.3861 ns |  1.67 |    0.03 |      - |     - |     - |         - |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     6.027 ns |   0.1226 ns |  0.1146 ns |  0.21 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    28.187 ns |   0.0936 ns |  0.0875 ns |  1.00 |    0.00 | 0.0127 |     - |     - |      40 B |
    | CisternLinq |             1 |    44.614 ns |   0.4914 ns |  0.4597 ns |  1.58 |    0.02 |      - |     - |     - |         - |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    28.800 ns |   0.4155 ns |  0.3886 ns |  0.25 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |            10 |   115.677 ns |   2.3332 ns |  2.3961 ns |  1.00 |    0.00 | 0.0126 |     - |     - |      40 B |
    | CisternLinq |            10 |   106.788 ns |   1.8191 ns |  1.7016 ns |  0.92 |    0.02 |      - |     - |     - |         - |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 2,422.825 ns |  38.2023 ns | 35.7344 ns |  0.29 |    0.01 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 8,487.874 ns | 107.8303 ns | 90.0432 ns |  1.00 |    0.00 |      - |     - |     - |      40 B |
    | CisternLinq |          1000 | 4,517.711 ns |  69.5579 ns | 65.0645 ns |  0.53 |    0.01 |      - |     - |     - |         - |
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
