using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     28.54 ns |   0.4659 ns |   0.4358 ns |  0.91 |    0.02 |      - |     - |     - |         - |
    | CisternForLoop |             0 |     42.37 ns |   0.3575 ns |   0.3344 ns |  1.36 |    0.02 | 0.0076 |     - |     - |      24 B |
    |     SystemLinq |             0 |     31.24 ns |   0.4256 ns |   0.3981 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     47.93 ns |   0.2668 ns |   0.2496 ns |  1.53 |    0.02 | 0.0178 |     - |     - |      56 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     80.56 ns |   1.1155 ns |   1.0434 ns |  1.01 |    0.01 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     67.63 ns |   0.7705 ns |   0.7207 ns |  0.85 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     79.59 ns |   0.4844 ns |   0.4294 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     95.84 ns |   1.2959 ns |   1.2122 ns |  1.20 |    0.02 | 0.0380 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    212.39 ns |   2.7407 ns |   2.5636 ns |  1.03 |    0.02 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    156.17 ns |   2.2793 ns |   1.9033 ns |  0.75 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    206.64 ns |   2.6330 ns |   2.4629 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    119.22 ns |   2.1691 ns |   2.0289 ns |  0.58 |    0.01 | 0.0379 |     - |     - |     120 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,796.31 ns | 126.5669 ns | 118.3908 ns |  1.07 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 | 10,139.33 ns | 238.9772 ns | 211.8470 ns |  0.73 |    0.02 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 13,885.71 ns |  82.0207 ns |  76.7222 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  3,355.18 ns |  33.5429 ns |  29.7349 ns |  0.24 |    0.00 | 0.0343 |     - |     - |     120 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Sum : SelectRangeBase
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
		public double SystemLinq() => System.Linq.Enumerable.Sum(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Sum(CisternNumbers);
	}
}
