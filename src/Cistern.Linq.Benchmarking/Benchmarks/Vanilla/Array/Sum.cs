using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2932 ns |  0.0098 ns |  0.0092 ns |  0.02 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    12.3747 ns |  0.0298 ns |  0.0264 ns |  1.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    26.9941 ns |  0.0620 ns |  0.0580 ns |  2.18 | 0.0102 |     - |     - |      32 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |             1 |     0.2885 ns |  0.0087 ns |  0.0082 ns |  0.01 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    20.8610 ns |  0.0288 ns |  0.0269 ns |  1.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    28.2053 ns |  0.0909 ns |  0.0851 ns |  1.35 | 0.0101 |     - |     - |      32 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |            10 |     6.0228 ns |  0.0308 ns |  0.0288 ns |  0.09 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    66.5754 ns |  0.2487 ns |  0.2204 ns |  1.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    36.9116 ns |  0.0576 ns |  0.0511 ns |  0.55 | 0.0101 |     - |     - |      32 B |
    |             |               |               |            |            |       |        |       |       |           |
    |     ForLoop |          1000 | 1,175.6591 ns |  2.8057 ns |  2.6244 ns |  0.23 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,009.6971 ns | 14.0764 ns | 13.1670 ns |  1.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 1,232.4168 ns |  2.0819 ns |  1.9474 ns |  0.25 | 0.0095 |     - |     - |      32 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Sum : VanillaArrayBase
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
		public double SystemLinq() => System.Linq.Enumerable.Sum(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Sum(Numbers);
	}
}
