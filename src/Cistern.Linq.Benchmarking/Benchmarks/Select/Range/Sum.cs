using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     28.82 ns |   0.3297 ns |   0.3084 ns |  0.92 |    0.01 |      - |     - |     - |         - |
    | CisternForLoop |             0 |     38.66 ns |   0.3249 ns |   0.2880 ns |  1.23 |    0.02 | 0.0076 |     - |     - |      24 B |
    |     SystemLinq |             0 |     31.48 ns |   0.3671 ns |   0.3434 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     47.95 ns |   0.4153 ns |   0.3885 ns |  1.52 |    0.02 | 0.0178 |     - |     - |      56 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.19 ns |   0.6919 ns |   0.6472 ns |  1.01 |    0.01 | 0.0304 |     - |     - |      96 B |
    | CisternForLoop |             1 |     94.69 ns |   0.8673 ns |   0.8113 ns |  1.18 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     80.00 ns |   0.8898 ns |   0.8324 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |     88.79 ns |   0.7159 ns |   0.6696 ns |  1.11 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    206.55 ns |   2.4328 ns |   2.2756 ns |  0.98 |    0.01 | 0.0303 |     - |     - |      96 B |
    | CisternForLoop |            10 |    188.71 ns |   1.9826 ns |   1.6555 ns |  0.90 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    210.00 ns |   2.5600 ns |   2.3946 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    156.09 ns |   3.6984 ns |   3.7980 ns |  0.74 |    0.02 | 0.0482 |     - |     - |     152 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 14,749.02 ns |  87.2413 ns |  68.1123 ns |  1.06 |    0.01 | 0.0153 |     - |     - |      96 B |
    | CisternForLoop |          1000 | 10,871.55 ns | 148.6758 ns | 139.0714 ns |  0.78 |    0.01 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 13,857.36 ns | 269.4472 ns | 252.0411 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  2,995.18 ns |  39.4562 ns |  36.9074 ns |  0.22 |    0.00 | 0.0458 |     - |     - |     152 B |
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
