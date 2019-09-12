using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     45.41 ns |   0.3247 ns |   0.3037 ns |  1.19 |    0.01 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     33.62 ns |   0.3422 ns |   0.3201 ns |  0.88 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     38.11 ns |   0.3833 ns |   0.3585 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     70.60 ns |   0.4909 ns |   0.4592 ns |  1.85 |    0.02 | 0.0228 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    111.09 ns |   0.9080 ns |   0.8493 ns |  1.28 |    0.01 | 0.0559 |     - |     - |     176 B |
    |  SystemForLoop |             1 |     82.70 ns |   0.4817 ns |   0.4506 ns |  0.95 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |             1 |     87.06 ns |   0.8568 ns |   0.8014 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    125.25 ns |   1.1755 ns |   1.0421 ns |  1.44 |    0.02 | 0.0532 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |    220.57 ns |   2.2405 ns |   1.9861 ns |  0.88 |    0.05 | 0.0558 |     - |     - |     176 B |
    |  SystemForLoop |            10 |    205.78 ns |   1.3262 ns |   1.2405 ns |  0.82 |    0.04 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |    254.03 ns |   5.1203 ns |   8.6946 ns |  1.00 |    0.00 | 0.0277 |     - |     - |      88 B |
    |    CisternLinq |            10 |    204.03 ns |   3.4336 ns |   3.2118 ns |  0.81 |    0.04 | 0.0532 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 10,890.19 ns | 187.3194 ns | 175.2186 ns |  0.71 |    0.02 | 0.0458 |     - |     - |     176 B |
    |  SystemForLoop |          1000 | 13,464.26 ns | 254.2417 ns | 237.8178 ns |  0.88 |    0.02 | 0.0153 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 15,301.18 ns | 192.1499 ns | 170.3359 ns |  1.00 |    0.00 |      - |     - |     - |      88 B |
    |    CisternLinq |          1000 |  5,488.18 ns |  39.3452 ns |  32.8550 ns |  0.36 |    0.00 | 0.0458 |     - |     - |     168 B |
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
