using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |              Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |-------------------- |-------------- |--------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |             ForLoop |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |          SystemLinq |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |  SystemLinqViaFirst |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |         CisternLinq |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternLinqViaFirst |             0 |            NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                     |               |               |            |            |       |         |        |       |       |           |
    |             ForLoop |             1 |     0.8857 ns |  0.0286 ns |  0.0268 ns |  0.02 |    0.00 |      - |     - |     - |         - |
    |          SystemLinq |             1 |    47.5659 ns |  0.3260 ns |  0.3049 ns |  1.00 |    0.00 | 0.0356 |     - |     - |     112 B |
    |  SystemLinqViaFirst |             1 |    65.4166 ns |  0.5924 ns |  0.5252 ns |  1.37 |    0.02 | 0.0355 |     - |     - |     112 B |
    |         CisternLinq |             1 |    48.9207 ns |  0.3668 ns |  0.3431 ns |  1.03 |    0.01 | 0.0381 |     - |     - |     120 B |
    | CisternLinqViaFirst |             1 |    62.1582 ns |  0.3885 ns |  0.3634 ns |  1.31 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                     |               |               |            |            |       |         |        |       |       |           |
    |             ForLoop |            10 |     6.6450 ns |  0.0660 ns |  0.0617 ns |  0.09 |    0.00 |      - |     - |     - |         - |
    |          SystemLinq |            10 |    71.8741 ns |  0.4154 ns |  0.3682 ns |  1.00 |    0.00 | 0.0355 |     - |     - |     112 B |
    |  SystemLinqViaFirst |            10 |    93.5916 ns |  0.6644 ns |  0.6215 ns |  1.30 |    0.01 | 0.0355 |     - |     - |     112 B |
    |         CisternLinq |            10 |    75.0067 ns |  0.4032 ns |  0.3771 ns |  1.04 |    0.01 | 0.0380 |     - |     - |     120 B |
    | CisternLinqViaFirst |            10 |    84.4973 ns |  0.4281 ns |  0.4004 ns |  1.18 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                     |               |               |            |            |       |         |        |       |       |           |
    |             ForLoop |          1000 |   695.7081 ns |  7.1765 ns |  5.9927 ns |  0.25 |    0.00 |      - |     - |     - |         - |
    |          SystemLinq |          1000 | 2,761.4715 ns | 21.2828 ns | 19.9080 ns |  1.00 |    0.00 | 0.0343 |     - |     - |     112 B |
    |  SystemLinqViaFirst |          1000 | 3,129.1724 ns | 25.7855 ns | 22.8582 ns |  1.13 |    0.01 | 0.0343 |     - |     - |     112 B |
    |         CisternLinq |          1000 | 2,831.1426 ns | 13.1795 ns | 12.3281 ns |  1.03 |    0.01 | 0.0343 |     - |     - |     120 B |
    | CisternLinqViaFirst |          1000 | 2,443.6893 ns | 24.2305 ns | 22.6652 ns |  0.88 |    0.01 | 0.0458 |     - |     - |     152 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class WhereBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			foreach (var n in Numbers)
			{
				if (n == NumberOfItems)
				{
					return n;
				}
			}

			throw new Exception("Not found!");
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			foreach (var item in System.Linq.Enumerable.Where(Numbers, n => n == NumberOfItems))
			{
				return item;
			}

			throw new Exception("Not found!");
		}

        [Benchmark]
        public double SystemLinqViaFirst()
        {
            return System.Linq.Enumerable.First(System.Linq.Enumerable.Where(Numbers, n => n == NumberOfItems));
        }

        [Benchmark]
		public double CisternLinq()
		{
			foreach (var item in Enumerable.Where(Numbers, n => n == NumberOfItems))
			{
				return item;
			}

			throw new Exception("Not found!");
		}

        [Benchmark]
        public double CisternLinqViaFirst()
        {
            return Cistern.Linq.Enumerable.First(Cistern.Linq.Enumerable.Where(Numbers, n => n == NumberOfItems));
        }

    }
}
