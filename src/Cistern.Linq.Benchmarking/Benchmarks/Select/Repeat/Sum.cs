using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     33.58 ns |   0.3016 ns |   0.2821 ns |  0.94 |    0.01 |      - |     - |     - |         - |
    | CisternForLoop |             0 |     43.68 ns |   0.2387 ns |   0.2233 ns |  1.23 |    0.01 | 0.0076 |     - |     - |      24 B |
    |     SystemLinq |             0 |     35.58 ns |   0.2749 ns |   0.2572 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     53.52 ns |   0.5815 ns |   0.5440 ns |  1.50 |    0.02 | 0.0178 |     - |     - |      56 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     81.77 ns |   0.5967 ns |   0.5582 ns |  0.91 |    0.01 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |             1 |     69.78 ns |   0.3398 ns |   0.3178 ns |  0.78 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     90.00 ns |   0.7931 ns |   0.7418 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |             1 |    102.54 ns |   0.2963 ns |   0.2474 ns |  1.14 |    0.01 | 0.0407 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    204.82 ns |   1.3048 ns |   1.2205 ns |  0.93 |    0.02 | 0.0279 |     - |     - |      88 B |
    | CisternForLoop |            10 |    166.18 ns |   0.7413 ns |   0.6571 ns |  0.75 |    0.02 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    222.73 ns |   4.4794 ns |   5.1585 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |    130.05 ns |   0.8826 ns |   0.8256 ns |  0.59 |    0.01 | 0.0405 |     - |     - |     128 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    |  SystemForLoop |          1000 | 13,063.75 ns |  57.8755 ns |  54.1368 ns |  0.99 |    0.01 | 0.0153 |     - |     - |      88 B |
    | CisternForLoop |          1000 |  8,602.43 ns | 120.3465 ns | 112.5722 ns |  0.65 |    0.01 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 13,232.99 ns | 107.3203 ns | 100.3874 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 |  3,764.22 ns |  36.9894 ns |  32.7902 ns |  0.28 |    0.00 | 0.0381 |     - |     - |     128 B |
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
