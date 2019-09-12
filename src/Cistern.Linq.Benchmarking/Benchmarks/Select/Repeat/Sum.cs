using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     33.36 ns |  0.3855 ns |  0.3606 ns |  0.94 |    0.01 |      - |     - |     - |         - |
    | CisternForLoop |             0 |     45.48 ns |  0.3500 ns |  0.3274 ns |  1.28 |    0.01 | 0.0076 |     - |     - |      24 B |
    |     SystemLinq |             0 |     35.53 ns |  0.2259 ns |  0.2113 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     54.63 ns |  0.3930 ns |  0.3484 ns |  1.54 |    0.02 | 0.0178 |     - |     - |      56 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.28 ns |  0.6407 ns |  0.5993 ns |  0.91 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |    113.35 ns |  0.7618 ns |  0.7126 ns |  1.26 |    0.01 | 0.0559 |     - |     - |     176 B |
    |     SystemLinq |             1 |     89.78 ns |  0.6463 ns |  0.6046 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    103.88 ns |  0.6725 ns |  0.5962 ns |  1.16 |    0.01 | 0.0483 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    204.41 ns |  1.5209 ns |  1.4226 ns |  0.90 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    218.79 ns |  1.3981 ns |  1.2393 ns |  0.96 |    0.01 | 0.0558 |     - |     - |     176 B |
    |     SystemLinq |            10 |    228.07 ns |  0.9418 ns |  0.8809 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    153.20 ns |  1.1096 ns |  0.9836 ns |  0.67 |    0.01 | 0.0482 |     - |     - |     152 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,072.11 ns | 71.8627 ns | 67.2204 ns |  0.94 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 | 10,704.88 ns | 77.3351 ns | 64.5783 ns |  0.77 |    0.00 | 0.0458 |     - |     - |     176 B |
    |     SystemLinq |          1000 | 13,902.67 ns | 75.4805 ns | 70.6045 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  3,616.66 ns | 21.0322 ns | 18.6445 ns |  0.26 |    0.00 | 0.0458 |     - |     - |     152 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRepeat_Sum : SelectRepeatBase
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
