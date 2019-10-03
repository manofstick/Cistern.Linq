using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     88.53 ns |   0.9647 ns |   0.9024 ns |  1.25 |    0.02 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             0 |     65.05 ns |   1.3311 ns |   1.9091 ns |  0.94 |    0.03 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             0 |     70.74 ns |   0.5919 ns |   0.5247 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             0 |     96.53 ns |   0.9800 ns |   0.9167 ns |  1.36 |    0.02 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     87.72 ns |   1.3110 ns |   1.1622 ns |  1.04 |    0.01 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             1 |     74.68 ns |   0.9027 ns |   0.8444 ns |  0.88 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             1 |     84.62 ns |   0.8560 ns |   0.8007 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             1 |    106.04 ns |   0.9170 ns |   0.8578 ns |  1.25 |    0.01 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    195.89 ns |   2.3166 ns |   2.1670 ns |  0.90 |    0.01 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |            10 |    198.66 ns |   1.8397 ns |   1.7208 ns |  0.91 |    0.01 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |            10 |    217.14 ns |   2.3452 ns |   2.1937 ns |  1.00 |    0.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |            10 |    201.18 ns |   1.7446 ns |   1.4568 ns |  0.92 |    0.01 | 0.0331 |     - |     - |     104 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 11,151.03 ns | 110.9733 ns | 103.8044 ns |  0.85 |    0.01 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 11,740.43 ns | 146.0951 ns | 129.5095 ns |  0.89 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,191.33 ns |  89.9461 ns |  84.1356 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,118.30 ns |  90.1898 ns |  84.3636 ns |  0.69 |    0.00 | 0.0305 |     - |     - |     104 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Aggreate : SelectEnumerableBase
    {
        [Benchmark]
        public double SystemForLoop()
        {
            double sum = 0;
            foreach (var n in SystemNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark]
        public double CisternForLoop()
        {
            double sum = 0;
            foreach (var n in CisternNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(SystemNumbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(CisternNumbers, 0.0, (a, c) => a + c);
    }
}
