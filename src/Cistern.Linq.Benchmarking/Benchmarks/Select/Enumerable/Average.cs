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
    |  SystemForLoop |             1 |     72.46 ns |   1.1607 ns |   1.0857 ns |  0.97 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     69.82 ns |   0.9030 ns |   0.8447 ns |  0.94 |    0.02 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     74.40 ns |   0.9749 ns |   0.9119 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     99.80 ns |   1.1271 ns |   0.9991 ns |  1.34 |    0.02 | 0.0660 |     - |     - |     208 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    200.38 ns |   1.3576 ns |   1.2699 ns |  1.01 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    197.54 ns |   1.8413 ns |   1.7224 ns |  1.00 |    0.02 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    198.54 ns |   2.4879 ns |   2.3272 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    202.64 ns |   2.2664 ns |   1.8925 ns |  1.02 |    0.01 | 0.0660 |     - |     - |     208 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,029.30 ns | 155.2351 ns | 145.2070 ns |  1.08 |    0.02 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,579.56 ns |  34.4311 ns |  28.7515 ns |  1.04 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 12,045.57 ns | 134.6991 ns | 125.9976 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 10,368.17 ns | 139.3491 ns | 130.3472 ns |  0.86 |    0.01 | 0.0610 |     - |     - |     208 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Average : SelectEnumerableBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            double sum = 0;
            int count = 0;
			foreach (var n in SystemNumbers)
			{
				sum += n;
                count++;
			}

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return sum / count;
		}

        [Benchmark]
        public double CisternForLoop()
        {
            double sum = 0;
            int count = 0;
            foreach (var n in CisternNumbers)
            {
                sum += n;
                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return sum / count;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Average(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Average(CisternNumbers);
	}
}
