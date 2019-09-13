using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     1.111 ns |  0.0339 ns |  0.0317 ns |  0.05 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    21.179 ns |  0.3469 ns |  0.3245 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    24.827 ns |  0.8536 ns |  0.9487 ns |  1.18 |    0.05 | 0.0127 |     - |     - |      40 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     6.052 ns |  0.0960 ns |  0.0898 ns |  0.09 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    65.115 ns |  0.8577 ns |  0.8023 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    44.114 ns |  0.6446 ns |  0.6030 ns |  0.68 |    0.01 | 0.0127 |     - |     - |      40 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,184.531 ns | 12.5326 ns | 11.7230 ns |  0.24 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 4,880.825 ns | 52.5109 ns | 49.1187 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 1,221.774 ns | 18.3805 ns | 17.1931 ns |  0.25 |    0.00 | 0.0114 |     - |     - |      40 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Average : VanillaArrayBase
    {
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Length == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
			}

			return sum / Numbers.Length;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Average(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Average(Numbers);
	}
}
