using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    18.19 ns |   0.2644 ns |   0.2473 ns |  0.59 |    0.01 |      - |     - |     - |         - |
    |  SystemForLoop |             0 |    27.32 ns |   0.4002 ns |   0.3743 ns |  0.88 |    0.02 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    30.94 ns |   0.3788 ns |   0.3543 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    38.72 ns |   0.5074 ns |   0.4746 ns |  1.25 |    0.02 |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    33.58 ns |   0.4013 ns |   0.3754 ns |  0.57 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |             1 |    58.55 ns |   0.8718 ns |   0.8155 ns |  0.99 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |             1 |    59.27 ns |   0.9162 ns |   0.8570 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |             1 |    66.05 ns |   1.3274 ns |   1.4754 ns |  1.12 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   103.16 ns |   0.9213 ns |   0.8618 ns |  0.72 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |            10 |   133.86 ns |   1.5642 ns |   1.4632 ns |  0.94 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |            10 |   142.73 ns |   2.1074 ns |   1.9713 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |            10 |   108.01 ns |   1.3651 ns |   1.2769 ns |  0.76 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 6,402.74 ns | 154.4465 ns | 231.1682 ns |  0.74 |    0.03 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |          1000 | 6,419.50 ns |  74.1578 ns |  69.3673 ns |  0.73 |    0.01 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |          1000 | 8,802.23 ns | 126.1384 ns | 117.9899 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |          1000 | 4,027.37 ns |  47.4569 ns |  44.3912 ns |  0.46 |    0.01 | 0.0153 |     - |     - |      48 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Aggreate : SelectArrayBase
	{
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

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(SystemNumbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(CisternNumbers, 0.0, (a, c) => a + c);
    }
}
