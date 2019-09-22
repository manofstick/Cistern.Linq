using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     53.17 ns |   0.2234 ns |   0.2090 ns |  0.72 | 0.0152 |     - |     - |      48 B |
    |  SystemForLoop |             0 |     71.79 ns |   0.8390 ns |   0.7848 ns |  0.97 | 0.0380 |     - |     - |     120 B |
    |     SystemLinq |             0 |     74.12 ns |   0.5523 ns |   0.5166 ns |  1.00 | 0.0380 |     - |     - |     120 B |
    |    CisternLinq |             0 |     76.18 ns |   0.4979 ns |   0.4657 ns |  1.03 | 0.0304 |     - |     - |      96 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |             1 |     89.97 ns |   0.5571 ns |   0.4938 ns |  0.87 | 0.0483 |     - |     - |     152 B |
    |  SystemForLoop |             1 |    100.92 ns |   0.5606 ns |   0.4969 ns |  0.98 | 0.0508 |     - |     - |     160 B |
    |     SystemLinq |             1 |    103.38 ns |   0.6026 ns |   0.5637 ns |  1.00 | 0.0508 |     - |     - |     160 B |
    |    CisternLinq |             1 |    142.98 ns |   0.9312 ns |   0.7776 ns |  1.38 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |            10 |    184.57 ns |   0.7183 ns |   0.6719 ns |  0.71 | 0.0482 |     - |     - |     152 B |
    |  SystemForLoop |            10 |    244.75 ns |   2.5873 ns |   2.4201 ns |  0.95 | 0.0505 |     - |     - |     160 B |
    |     SystemLinq |            10 |    258.43 ns |   2.5197 ns |   2.3570 ns |  1.00 | 0.0505 |     - |     - |     160 B |
    |    CisternLinq |            10 |    209.09 ns |   1.2462 ns |   1.0407 ns |  0.81 | 0.0634 |     - |     - |     200 B |
    |                |               |              |             |             |       |        |       |       |           |
    | CisternForLoop |          1000 | 10,004.60 ns |  80.5641 ns |  75.3597 ns |  0.65 | 0.0458 |     - |     - |     152 B |
    |  SystemForLoop |          1000 | 14,723.66 ns |  74.6207 ns |  66.1493 ns |  0.95 | 0.0458 |     - |     - |     160 B |
    |     SystemLinq |          1000 | 15,453.91 ns | 122.6237 ns | 114.7023 ns |  1.00 | 0.0305 |     - |     - |     160 B |
    |    CisternLinq |          1000 |  6,910.78 ns |  70.3799 ns |  65.8334 ns |  0.45 | 0.0610 |     - |     - |     200 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRange_Aggreate : WhereSelectRangeBase
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
