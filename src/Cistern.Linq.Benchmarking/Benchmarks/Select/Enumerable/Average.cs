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
    |  SystemForLoop |             1 |     73.08 ns |   1.1180 ns |   1.0458 ns |  0.98 |    0.02 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     70.62 ns |   0.6098 ns |   0.5405 ns |  0.95 |    0.02 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     74.61 ns |   1.1413 ns |   1.0676 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     88.87 ns |   1.3753 ns |   1.1484 ns |  1.19 |    0.02 | 0.0457 |     - |     - |     144 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    200.93 ns |   2.8088 ns |   2.6274 ns |  1.01 |    0.02 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    199.78 ns |   2.0557 ns |   1.9229 ns |  1.00 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    199.63 ns |   2.6431 ns |   2.4724 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    171.91 ns |   1.7819 ns |   1.6668 ns |  0.86 |    0.01 | 0.0455 |     - |     - |     144 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,095.99 ns | 155.1341 ns | 145.1125 ns |  1.05 |    0.03 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,952.91 ns |  74.1717 ns |  65.7513 ns |  1.04 |    0.03 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 12,517.08 ns | 250.1986 ns | 297.8437 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  8,688.42 ns | 169.9072 ns | 220.9274 ns |  0.69 |    0.03 | 0.0305 |     - |     - |     144 B |
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
