using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.13 ns |   0.6200 ns |   0.5800 ns |  0.97 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     79.68 ns |   0.6910 ns |   0.6464 ns |  0.95 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     84.05 ns |   0.6007 ns |   0.5619 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     97.02 ns |   0.5650 ns |   0.5285 ns |  1.15 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    237.43 ns |   1.7020 ns |   1.4212 ns |  0.93 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    236.77 ns |   1.5490 ns |   1.3731 ns |  0.93 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    254.56 ns |   1.9410 ns |   1.8156 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    192.88 ns |   1.8021 ns |   1.5975 ns |  0.76 |    0.01 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 15,588.23 ns | 123.6755 ns | 115.6861 ns |  0.90 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 15,549.78 ns | 125.4359 ns | 117.3328 ns |  0.89 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 17,376.50 ns |  92.0686 ns |  86.1210 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,243.66 ns | 126.7291 ns | 118.5425 ns |  0.53 |    0.01 | 0.0305 |     - |     - |     136 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Min : SelectEnumerableBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            var min = double.PositiveInfinity;
            var noData = true;
			foreach (var n in SystemNumbers)
			{
                noData = false;
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

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return min;
		}

        [Benchmark]
        public double CisternForLoop()
        {
            var min = double.PositiveInfinity;
            var noData = true;
            foreach (var n in CisternNumbers)
            {
                noData = false;
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

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return min;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Min(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Min(CisternNumbers);
	}
}
