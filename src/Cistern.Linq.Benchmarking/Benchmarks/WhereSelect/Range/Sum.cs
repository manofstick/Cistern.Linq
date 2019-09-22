using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     72.06 ns |   0.4516 ns |   0.4224 ns |  1.02 | 0.0380 |     - |     - |     120 B |
    | CisternForLoop |             0 |     53.25 ns |   0.3049 ns |   0.2852 ns |  0.75 | 0.0152 |     - |     - |      48 B |
    |     SystemLinq |             0 |     70.86 ns |   0.7192 ns |   0.6727 ns |  1.00 | 0.0380 |     - |     - |     120 B |
    |    CisternLinq |             0 |     61.28 ns |   0.4709 ns |   0.4405 ns |  0.86 | 0.0254 |     - |     - |      80 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |             1 |    100.35 ns |   0.4979 ns |   0.4657 ns |  0.97 | 0.0508 |     - |     - |     160 B |
    | CisternForLoop |             1 |     90.15 ns |   0.6038 ns |   0.5648 ns |  0.88 | 0.0483 |     - |     - |     152 B |
    |     SystemLinq |             1 |    102.96 ns |   0.4126 ns |   0.3860 ns |  1.00 | 0.0508 |     - |     - |     160 B |
    |    CisternLinq |             1 |    126.03 ns |   1.4157 ns |   1.2550 ns |  1.22 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |            10 |    244.46 ns |   2.4902 ns |   2.3293 ns |  0.99 | 0.0505 |     - |     - |     160 B |
    | CisternForLoop |            10 |    186.04 ns |   2.1615 ns |   1.9161 ns |  0.76 | 0.0482 |     - |     - |     152 B |
    |     SystemLinq |            10 |    245.83 ns |   2.6609 ns |   2.4890 ns |  1.00 | 0.0505 |     - |     - |     160 B |
    |    CisternLinq |            10 |    162.61 ns |   1.9023 ns |   1.5885 ns |  0.66 | 0.0584 |     - |     - |     184 B |
    |                |               |              |             |             |       |        |       |       |           |
    |  SystemForLoop |          1000 | 14,715.87 ns |  66.2567 ns |  61.9765 ns |  1.05 | 0.0458 |     - |     - |     160 B |
    | CisternForLoop |          1000 | 10,017.57 ns | 123.5596 ns | 109.5324 ns |  0.72 | 0.0458 |     - |     - |     152 B |
    |     SystemLinq |          1000 | 14,000.15 ns |  62.2269 ns |  55.1626 ns |  1.00 | 0.0458 |     - |     - |     160 B |
    |    CisternLinq |          1000 |  4,585.73 ns |  33.3683 ns |  31.2128 ns |  0.33 | 0.0534 |     - |     - |     184 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRange_Sum : WhereSelectRangeBase
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
