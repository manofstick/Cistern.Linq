using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     29.71 ns |   0.4866 ns |   0.4552 ns |  1.19 |    0.02 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     23.73 ns |   0.3650 ns |   0.3414 ns |  0.95 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     24.88 ns |   0.4631 ns |   0.4332 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     56.30 ns |   1.0859 ns |   1.0665 ns |  2.26 |    0.07 | 0.0076 |     - |     - |      24 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |     56.41 ns |   1.0089 ns |   0.9438 ns |  0.64 |    0.01 | 0.0306 |     - |     - |      96 B |
    |  SystemForLoop |             1 |     86.24 ns |   1.1950 ns |   1.0593 ns |  0.98 |    0.02 | 0.0280 |     - |     - |      88 B |
    |     SystemLinq |             1 |     87.78 ns |   1.3708 ns |   1.2152 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |     90.93 ns |   0.9120 ns |   0.8085 ns |  1.04 |    0.02 | 0.0305 |     - |     - |      96 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |    129.61 ns |   2.0818 ns |   1.9473 ns |  0.58 |    0.02 | 0.0305 |     - |     - |      96 B |
    |  SystemForLoop |            10 |    193.53 ns |   1.9515 ns |   1.8255 ns |  0.87 |    0.02 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    223.45 ns |   4.3590 ns |   4.2811 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    126.41 ns |   2.3037 ns |   2.1549 ns |  0.57 |    0.02 | 0.0305 |     - |     - |      96 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 |  7,408.74 ns | 114.8306 ns | 101.7944 ns |  0.59 |    0.01 | 0.0305 |     - |     - |      96 B |
    |  SystemForLoop |          1000 | 10,631.13 ns | 207.9267 ns | 194.4947 ns |  0.85 |    0.02 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 12,442.03 ns | 193.3386 ns | 180.8490 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  4,711.96 ns |  18.1051 ns |  16.9355 ns |  0.38 |    0.01 | 0.0305 |     - |     - |      96 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRepeat_Aggreate : SelectRepeatBase
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
