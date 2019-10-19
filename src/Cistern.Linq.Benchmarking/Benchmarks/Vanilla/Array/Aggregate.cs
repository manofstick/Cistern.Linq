using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |       Error |      StdDev |        Median | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|------------:|------------:|--------------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2918 ns |   0.0083 ns |   0.0077 ns |     0.2937 ns |  0.03 |      - |     - |     - |         - |
    |  SystemLinq |             0 |     9.5696 ns |   0.0325 ns |   0.0304 ns |     9.5621 ns |  1.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    38.1405 ns |   0.1163 ns |   0.0971 ns |    38.1331 ns |  3.99 |      - |     - |     - |         - |
    |             |               |               |             |             |               |       |        |       |       |           |
    |     ForLoop |             1 |     0.3436 ns |   0.0063 ns |   0.0059 ns |     0.3446 ns |  0.02 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    21.8673 ns |   0.1024 ns |   0.0957 ns |    21.8626 ns |  1.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    47.1529 ns |   0.1413 ns |   0.1253 ns |    47.0911 ns |  2.16 |      - |     - |     - |         - |
    |             |               |               |             |             |               |       |        |       |       |           |
    |     ForLoop |            10 |     4.1247 ns |   0.0160 ns |   0.0150 ns |     4.1241 ns |  0.04 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    95.1824 ns |   0.3778 ns |   0.3534 ns |    95.2266 ns |  1.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    68.4785 ns |   0.1370 ns |   0.1070 ns |    68.5061 ns |  0.72 |      - |     - |     - |         - |
    |             |               |               |             |             |               |       |        |       |       |           |
    |     ForLoop |          1000 | 1,251.2005 ns |   4.1087 ns |   3.8433 ns | 1,250.8064 ns |  0.16 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,130.8506 ns | 142.1031 ns | 393.7670 ns | 6,869.6453 ns |  1.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 2,158.2476 ns |   7.0185 ns |   6.5651 ns | 2,158.4448 ns |  0.28 |      - |     - |     - |         - |
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
