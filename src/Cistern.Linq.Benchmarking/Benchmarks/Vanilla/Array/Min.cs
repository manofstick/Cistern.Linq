using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |             1 |     1.553 ns |   0.0270 ns |   0.0253 ns |  0.07 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |             1 |    21.636 ns |   0.1406 ns |   0.1315 ns |  1.00 |    0.00 | 0.0102 |     - |     - |      32 B |
    | CisternLinq |             1 |    32.073 ns |   0.2259 ns |   0.2113 ns |  1.48 |    0.01 | 0.0101 |     - |     - |      32 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |            10 |    13.203 ns |   0.1084 ns |   0.1014 ns |  0.14 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |            10 |    92.285 ns |   0.6556 ns |   0.6133 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |            10 |    45.557 ns |   0.3162 ns |   0.2957 ns |  0.49 |    0.00 | 0.0101 |     - |     - |      32 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |          1000 | 1,298.621 ns |   6.9372 ns |   6.4890 ns |  0.16 |    0.00 |      - |     - |     - |         - |
    |  SystemLinq |          1000 | 7,990.021 ns | 139.4630 ns | 123.6303 ns |  1.00 |    0.00 |      - |     - |     - |      32 B |
    | CisternLinq |          1000 | 1,624.513 ns |   6.8535 ns |   6.4108 ns |  0.20 |    0.00 | 0.0095 |     - |     - |      32 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Min : VanillaArrayBase
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
