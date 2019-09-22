using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     67.83 ns |  0.7391 ns |  0.6913 ns |  0.95 |    0.01 | 0.0330 |     - |     - |     104 B |
    | CisternForLoop |             1 |     76.92 ns |  0.6871 ns |  0.6427 ns |  1.08 |    0.01 | 0.0380 |     - |     - |     120 B |
    |     SystemLinq |             1 |     71.45 ns |  0.5817 ns |  0.5441 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |     91.00 ns |  0.5879 ns |  0.5499 ns |  1.27 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    172.85 ns |  1.1148 ns |  1.0428 ns |  0.90 |    0.01 | 0.0329 |     - |     - |     104 B |
    | CisternForLoop |            10 |    175.76 ns |  1.0416 ns |  0.9743 ns |  0.92 |    0.01 | 0.0379 |     - |     - |     120 B |
    |     SystemLinq |            10 |    191.27 ns |  1.0592 ns |  0.9908 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    136.90 ns |  1.4284 ns |  1.3362 ns |  0.72 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  9,551.04 ns | 71.0797 ns | 66.4880 ns |  0.81 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 |  9,825.87 ns | 58.0956 ns | 54.3426 ns |  0.83 |    0.01 | 0.0305 |     - |     - |     120 B |
    |     SystemLinq |          1000 | 11,854.69 ns | 96.6298 ns | 90.3876 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  4,817.64 ns | 32.3663 ns | 30.2754 ns |  0.41 |    0.00 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectArray_Min : WhereSelectArrayBase
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
