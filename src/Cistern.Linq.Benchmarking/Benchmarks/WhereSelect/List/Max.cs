using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.WhereSelect.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    | CisternForLoop |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |     SystemLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |    CisternLinq |             0 |           NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     74.92 ns |  0.6043 ns |  0.5653 ns |  0.90 |    0.01 | 0.0483 |     - |     - |     152 B |
    | CisternForLoop |             1 |     88.38 ns |  0.6401 ns |  0.5988 ns |  1.07 |    0.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     82.95 ns |  0.6541 ns |  0.5798 ns |  1.00 |    0.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    106.72 ns |  0.4219 ns |  0.3523 ns |  1.29 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    185.37 ns |  0.9867 ns |  0.8747 ns |  0.92 |    0.01 | 0.0482 |     - |     - |     152 B |
    | CisternForLoop |            10 |    198.30 ns |  1.2708 ns |  0.9922 ns |  0.98 |    0.01 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    201.87 ns |  1.5724 ns |  1.3938 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    158.26 ns |  0.9947 ns |  0.8818 ns |  0.78 |    0.01 | 0.0584 |     - |     - |     184 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 10,492.16 ns | 61.8210 ns | 57.8274 ns |  0.91 |    0.01 | 0.0458 |     - |     - |     152 B |
    | CisternForLoop |          1000 | 10,427.12 ns | 64.6642 ns | 60.4869 ns |  0.90 |    0.01 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 11,523.55 ns | 88.0944 ns | 78.0934 ns |  1.00 |    0.00 | 0.0458 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  5,497.83 ns | 34.3482 ns | 30.4488 ns |  0.48 |    0.00 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectList_Max : WhereSelectListBase
    {
		[Benchmark]
		public double SystemForLoop()
		{
            var noData = true;

            double max = double.NaN;

            using (var e = SystemNumbers.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    noData = false;
                    max = e.Current;
                    if (!double.IsNaN(max))
                        break;
                }
                while (e.MoveNext())
                {
                    var n = e.Current;
                    if (n > max)
                        max = n;
                }
            }

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return max;
        }

        [Benchmark]
        public double CisternForLoop()
        {
            var noData = true;

            double max = double.NaN;

            using (var e = CisternNumbers.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    noData = false;
                    max = e.Current;
                    if (!double.IsNaN(max))
                        break;
                }
                while (e.MoveNext())
                {
                    var n = e.Current;
                    if (n > max)
                        max = n;
                }
            }

            if (noData)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return max;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Max(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Max(CisternNumbers);
	}
}
