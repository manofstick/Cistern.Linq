using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     38.49 ns |   0.4737 ns |   0.4431 ns |  1.20 |    0.02 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     28.91 ns |   0.3578 ns |   0.3346 ns |  0.90 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     31.97 ns |   0.4173 ns |   0.3903 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     61.45 ns |   0.9458 ns |   0.8384 ns |  1.92 |    0.04 | 0.0228 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |     94.38 ns |   1.1249 ns |   1.0523 ns |  1.15 |    0.02 | 0.0559 |     - |     - |     176 B |
    |  SystemForLoop |             1 |     82.88 ns |   0.3502 ns |   0.2924 ns |  1.01 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     82.04 ns |   1.0016 ns |   0.9369 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |    106.72 ns |   0.3127 ns |   0.2925 ns |  1.30 |    0.02 | 0.0533 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |    192.58 ns |   2.2781 ns |   2.1310 ns |  0.91 |    0.01 | 0.0558 |     - |     - |     176 B |
    |  SystemForLoop |            10 |    207.86 ns |   2.3826 ns |   2.2287 ns |  0.99 |    0.02 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    210.79 ns |   3.1275 ns |   2.9255 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    158.84 ns |   2.5425 ns |   2.2538 ns |  0.75 |    0.02 | 0.0532 |     - |     - |     168 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 |  9,186.22 ns | 140.8651 ns | 131.7653 ns |  0.60 |    0.02 | 0.0458 |     - |     - |     176 B |
    |  SystemForLoop |          1000 | 12,794.68 ns |  57.5505 ns |  53.8328 ns |  0.84 |    0.03 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 15,205.28 ns | 310.5696 ns | 455.2289 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  5,807.64 ns |  36.1572 ns |  32.0524 ns |  0.38 |    0.01 | 0.0458 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectRange_Aggreate : SelectRangeBase
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
