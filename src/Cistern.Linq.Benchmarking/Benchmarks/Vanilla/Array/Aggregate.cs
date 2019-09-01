using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |--------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     0.2926 ns |  0.0119 ns |  0.0112 ns |  0.02 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             0 |    14.6364 ns |  0.0638 ns |  0.0566 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    44.2824 ns |  0.1676 ns |  0.1486 ns |  3.03 |    0.01 | 0.0152 |     - |     - |      48 B |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     0.3161 ns |  0.0125 ns |  0.0110 ns |  0.01 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    24.0854 ns |  0.1415 ns |  0.1255 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    48.3676 ns |  0.3450 ns |  0.3227 ns |  2.01 |    0.02 | 0.0152 |     - |     - |      48 B |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     6.4565 ns |  0.0364 ns |  0.0323 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    94.0138 ns |  0.3425 ns |  0.3203 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    66.4970 ns |  0.3825 ns |  0.3578 ns |  0.71 |    0.00 | 0.0151 |     - |     - |      48 B |
    |             |               |               |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,258.8319 ns |  6.7944 ns |  6.3555 ns |  0.16 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,775.4790 ns | 27.1579 ns | 25.4035 ns |  1.00 |    0.00 |      - |     - |     - |      32 B |
    | CisternLinq |          1000 | 1,997.1233 ns | 11.5739 ns | 10.8262 ns |  0.26 |    0.00 | 0.0114 |     - |     - |      48 B |
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
