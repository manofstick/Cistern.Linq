using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     43.74 ns |   0.3617 ns |   0.3383 ns |  1.15 |    0.01 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     33.59 ns |   0.3368 ns |   0.3150 ns |  0.88 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     38.15 ns |   0.2974 ns |   0.2782 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     68.83 ns |   0.4889 ns |   0.4573 ns |  1.80 |    0.02 | 0.0228 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |     69.93 ns |   0.8365 ns |   0.7825 ns |  0.81 |    0.01 | 0.0304 |     - |     - |      96 B |
    |  SystemForLoop |             1 |     81.86 ns |   0.6822 ns |   0.6047 ns |  0.94 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     86.79 ns |   0.6326 ns |   0.5918 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    120.74 ns |   1.1334 ns |   1.0047 ns |  1.39 |    0.02 | 0.0455 |     - |     - |     144 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |    163.59 ns |   1.0516 ns |   0.9837 ns |  0.70 |    0.01 | 0.0303 |     - |     - |      96 B |
    |  SystemForLoop |            10 |    205.17 ns |   1.7395 ns |   1.6272 ns |  0.87 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    234.54 ns |   1.2940 ns |   1.2104 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    173.51 ns |   1.7193 ns |   1.5242 ns |  0.74 |    0.01 | 0.0455 |     - |     - |     144 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 |  8,498.89 ns |  49.2599 ns |  43.6676 ns |  0.56 |    0.01 | 0.0153 |     - |     - |      96 B |
    |  SystemForLoop |          1000 | 13,061.44 ns | 121.6772 ns | 113.8169 ns |  0.86 |    0.01 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 15,150.62 ns | 114.5945 ns | 107.1918 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  5,289.88 ns |  55.6656 ns |  49.3461 ns |  0.35 |    0.00 | 0.0381 |     - |     - |     144 B |
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
