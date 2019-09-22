using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Repeat
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     55.38 ns |   0.2601 ns |   0.2433 ns |  0.73 | 0.0152 |     - |     - |      48 B |
    |  SystemForLoop |             0 |     74.46 ns |   0.7096 ns |   0.6638 ns |  0.98 | 0.0380 |     - |     - |     120 B |
    |     SystemLinq |             0 |     75.93 ns |   0.7049 ns |   0.6594 ns |  1.00 | 0.0380 |     - |     - |     120 B |
    |    CisternLinq |             0 |     75.27 ns |   0.5116 ns |   0.4786 ns |  0.99 | 0.0304 |     - |     - |      96 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |             1 |     93.87 ns |   0.4556 ns |   0.4262 ns |  0.97 | 0.0533 |     - |     - |     168 B |
    |  SystemForLoop |             1 |     98.12 ns |   0.3197 ns |   0.2990 ns |  1.01 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |     97.20 ns |   0.4665 ns |   0.3895 ns |  1.00 | 0.0483 |     - |     - |     152 B |
    |    CisternLinq |             1 |    139.44 ns |   1.2501 ns |   1.0439 ns |  1.43 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |            10 |    185.46 ns |   1.1921 ns |   1.1151 ns |  0.75 | 0.0532 |     - |     - |     168 B |
    |  SystemForLoop |            10 |    222.21 ns |   1.0542 ns |   0.9345 ns |  0.90 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    246.14 ns |   2.6376 ns |   2.2025 ns |  1.00 | 0.0482 |     - |     - |     152 B |
    |    CisternLinq |            10 |    201.35 ns |   1.0542 ns |   0.9346 ns |  0.82 | 0.0684 |     - |     - |     216 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |          1000 |  9,382.05 ns |  69.5473 ns |  65.0546 ns |  0.61 | 0.0458 |     - |     - |     168 B |
    |  SystemForLoop |          1000 | 12,518.69 ns |  65.7311 ns |  58.2689 ns |  0.81 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 15,467.20 ns | 140.9304 ns | 131.8264 ns |  1.00 | 0.0305 |     - |     - |     152 B |
    |    CisternLinq |          1000 |  6,658.06 ns |  98.4180 ns |  82.1835 ns |  0.43 | 0.0610 |     - |     - |     216 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRepeat_Aggreate : WhereSelectRepeatBase
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
