using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2902 ns |  0.0073 ns |  0.0068 ns |  0.02 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    13.7485 ns |  0.0376 ns |  0.0333 ns |  1.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    35.4010 ns |  0.1326 ns |  0.1241 ns |  2.57 | 0.0152 |     - |     - |      48 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |             1 |     0.2899 ns |  0.0070 ns |  0.0066 ns |  0.01 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    22.4716 ns |  0.0590 ns |  0.0523 ns |  1.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    38.2939 ns |  0.0721 ns |  0.0639 ns |  1.70 | 0.0152 |     - |     - |      48 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |            10 |     6.0197 ns |  0.0184 ns |  0.0172 ns |  0.07 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    87.8302 ns |  0.2279 ns |  0.2021 ns |  1.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    59.6775 ns |  0.3270 ns |  0.3059 ns |  0.68 | 0.0151 |     - |     - |      48 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |          1000 | 1,174.4642 ns |  2.4287 ns |  1.8962 ns |  0.16 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,273.6308 ns | 13.7677 ns | 12.8783 ns |  1.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 1,861.6622 ns |  3.9504 ns |  3.6952 ns |  0.26 | 0.0134 |     - |     - |      48 B |
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
