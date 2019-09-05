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
    |  SystemForLoop |             1 |     72.21 ns |   0.9576 ns |   0.8958 ns |  0.97 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     69.79 ns |   1.0592 ns |   0.9907 ns |  0.94 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     74.22 ns |   1.0760 ns |   1.0065 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    103.69 ns |   1.2136 ns |   1.1352 ns |  1.40 |    0.02 | 0.0635 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    209.05 ns |   2.6589 ns |   2.4872 ns |  0.95 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    209.51 ns |   2.4255 ns |   2.2688 ns |  0.95 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    220.78 ns |   0.5196 ns |   0.4057 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    213.17 ns |   2.5175 ns |   2.3549 ns |  0.96 |    0.01 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,715.40 ns | 111.1218 ns |  92.7918 ns |  0.89 |    0.02 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 13,974.79 ns | 181.7735 ns | 170.0310 ns |  0.91 |    0.02 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 15,411.03 ns | 214.6748 ns | 200.8069 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 | 10,226.39 ns | 116.2886 ns | 108.7765 ns |  0.66 |    0.01 | 0.0610 |     - |     - |     200 B |
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
