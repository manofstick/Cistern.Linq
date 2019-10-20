using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |          NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |             1 |     1.595 ns |   0.0248 ns |  0.0232 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    21.660 ns |   0.1061 ns |  0.0992 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    27.885 ns |   0.1704 ns |  0.1511 ns |  1.29 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |            10 |    13.422 ns |   0.0989 ns |  0.0926 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    93.008 ns |   0.4334 ns |  0.4054 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    38.150 ns |   0.1998 ns |  0.1869 ns |  0.41 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |             |            |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,311.083 ns |   4.5158 ns |  4.2241 ns |  0.16 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 8,034.498 ns | 104.2097 ns | 92.3792 ns |  1.00 |    0.00 |      - |     - |     - |      32 B |
    | CisternLinq |          1000 |   359.401 ns |   2.9055 ns |  2.5757 ns |  0.04 |    0.00 | 0.0100 |     - |     - |      32 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Min : VanillaDoubleArrayBase
    {
		[Benchmark]
		public double ForLoop()
		{
            if (Numbers.Length == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            double min = 0;
			foreach (var n in Numbers)
			{
				if (n < min)
				{
					min = n;
				}
                else if (double.IsNaN(n))
                {
                    min = double.NaN;
                    break;
                }
			}
			
			return min;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Min(Numbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(Numbers);
	}
}
