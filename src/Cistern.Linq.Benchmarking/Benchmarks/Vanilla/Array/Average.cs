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
    |     ForLoop |             1 |     1.210 ns |  0.0176 ns |  0.0165 ns |  0.06 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    21.495 ns |  0.1428 ns |  0.1336 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    30.252 ns |  0.1503 ns |  0.1332 ns |  1.41 |    0.01 | 0.0127 |     - |     - |      40 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |            10 |     6.470 ns |  0.0381 ns |  0.0357 ns |  0.09 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    69.387 ns |  0.3265 ns |  0.2894 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    56.221 ns |  0.4656 ns |  0.4355 ns |  0.81 |    0.01 | 0.0127 |     - |     - |      40 B |
    |             |               |              |            |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,259.924 ns |  4.6644 ns |  4.3631 ns |  0.24 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 5,236.866 ns | 29.7860 ns | 27.8618 ns |  1.00 |    0.00 | 0.0076 |     - |     - |      32 B |
    | CisternLinq |          1000 | 2,928.027 ns | 14.7535 ns | 13.8005 ns |  0.56 |    0.00 | 0.0114 |     - |     - |      40 B |
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
