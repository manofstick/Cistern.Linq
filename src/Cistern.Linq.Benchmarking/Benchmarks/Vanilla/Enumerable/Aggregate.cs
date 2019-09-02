using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |    16.09 ns |  0.0499 ns |  0.0443 ns |  0.84 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             0 |    19.12 ns |  0.0535 ns |  0.0447 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             0 |    71.89 ns |  0.4267 ns |  0.3782 ns |  3.76 |    0.02 | 0.0304 |     - |     - |      96 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |    21.47 ns |  0.0521 ns |  0.0487 ns |  0.81 |    0.00 | 0.0152 |     - |     - |      48 B |
    |  SystemLinq |             1 |    26.45 ns |  0.0905 ns |  0.0802 ns |  1.00 |    0.00 | 0.0152 |     - |     - |      48 B |
    | CisternLinq |             1 |    86.25 ns |  0.2077 ns |  0.1943 ns |  3.26 |    0.01 | 0.0304 |     - |     - |      96 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    75.82 ns |  0.1287 ns |  0.1204 ns |  0.77 |    0.00 | 0.0151 |     - |     - |      48 B |
    |  SystemLinq |            10 |    98.93 ns |  0.3142 ns |  0.2939 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |            10 |   152.99 ns |  1.4115 ns |  1.3203 ns |  1.55 |    0.01 | 0.0303 |     - |     - |      96 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 6,075.10 ns | 13.3440 ns | 12.4820 ns |  0.87 |    0.01 | 0.0076 |     - |     - |      48 B |
    |  SystemLinq |          1000 | 7,003.32 ns | 66.3781 ns | 51.8236 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      48 B |
    | CisternLinq |          1000 | 7,039.06 ns | 15.5913 ns | 14.5841 ns |  1.01 |    0.01 | 0.0229 |     - |     - |      96 B |
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
