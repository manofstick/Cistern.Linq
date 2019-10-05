using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2207 ns |  0.0246 ns |  0.0206 ns |  0.02 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             0 |     9.2281 ns |  0.1281 ns |  0.1198 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    32.9050 ns |  0.3628 ns |  0.3216 ns |  3.57 |    0.03 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     0.2809 ns |  0.0156 ns |  0.0146 ns |  0.01 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    19.8459 ns |  0.1988 ns |  0.1860 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    40.2410 ns |  0.8144 ns |  0.9378 ns |  2.04 |    0.06 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     3.7619 ns |  0.0448 ns |  0.0397 ns |  0.04 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    87.0499 ns |  0.8935 ns |  0.8357 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    61.2523 ns |  0.8189 ns |  0.7660 ns |  0.70 |    0.01 |      - |     - |     - |         - |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,154.1803 ns | 13.7799 ns | 12.8897 ns |  0.18 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 6,307.8071 ns | 83.9675 ns | 74.4350 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 2,187.9258 ns | 86.2644 ns | 80.6918 ns |  0.35 |    0.02 |      - |     - |     - |         - |
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
