using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |          NA |          NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     93.50 ns |   0.5536 ns |   0.5178 ns |  0.96 |    0.01 | 0.0533 |     - |     - |     168 B |
    | CisternForLoop |             1 |    109.12 ns |   0.5505 ns |   0.4880 ns |  1.12 |    0.01 | 0.0533 |     - |     - |     168 B |
    |     SystemLinq |             1 |     97.81 ns |   0.4154 ns |   0.3682 ns |  1.00 |    0.00 | 0.0533 |     - |     - |     168 B |
    |    CisternLinq |             1 |    127.06 ns |   1.7093 ns |   1.5152 ns |  1.30 |    0.01 | 0.0660 |     - |     - |     208 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    234.92 ns |   2.2648 ns |   2.1185 ns |  0.98 |    0.01 | 0.0529 |     - |     - |     168 B |
    | CisternForLoop |            10 |    251.78 ns |   2.1340 ns |   1.9962 ns |  1.05 |    0.02 | 0.0529 |     - |     - |     168 B |
    |     SystemLinq |            10 |    240.35 ns |   2.4356 ns |   2.2783 ns |  1.00 |    0.00 | 0.0529 |     - |     - |     168 B |
    |    CisternLinq |            10 |    214.14 ns |   0.7918 ns |   0.7019 ns |  0.89 |    0.01 | 0.0660 |     - |     - |     208 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,798.16 ns | 125.1596 ns | 117.0744 ns |  1.01 |    0.01 | 0.0305 |     - |     - |     168 B |
    | CisternForLoop |          1000 | 15,100.35 ns |  52.3988 ns |  49.0139 ns |  1.03 |    0.00 | 0.0458 |     - |     - |     168 B |
    |     SystemLinq |          1000 | 14,677.91 ns |  43.8499 ns |  36.6167 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     168 B |
    |    CisternLinq |          1000 |  8,442.28 ns |  72.9331 ns |  68.2217 ns |  0.58 |    0.00 | 0.0610 |     - |     - |     208 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectEnumerable_Average : WhereSelectEnumerableBase
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
