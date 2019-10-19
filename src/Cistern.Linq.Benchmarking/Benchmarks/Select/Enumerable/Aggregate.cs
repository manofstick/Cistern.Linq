using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     77.64 ns |  0.5060 ns |  0.4733 ns |  1.01 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             0 |     75.74 ns |  0.3864 ns |  0.3614 ns |  0.98 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             0 |     77.00 ns |  0.3894 ns |  0.3642 ns |  1.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             0 |    115.85 ns |  0.3499 ns |  0.3273 ns |  1.50 | 0.0331 |     - |     - |     104 B |
    |                |               |              |            |            |       |        |       |       |           |
    |  SystemForLoop |             1 |     88.11 ns |  0.4481 ns |  0.3972 ns |  0.99 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |             1 |     87.31 ns |  0.2773 ns |  0.2458 ns |  0.98 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |             1 |     88.99 ns |  0.5652 ns |  0.4720 ns |  1.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |             1 |    130.35 ns |  0.6459 ns |  0.6041 ns |  1.46 | 0.0331 |     - |     - |     104 B |
    |                |               |              |            |            |       |        |       |       |           |
    |  SystemForLoop |            10 |    213.42 ns |  0.6682 ns |  0.6250 ns |  0.92 | 0.0331 |     - |     - |     104 B |
    | CisternForLoop |            10 |    219.72 ns |  0.8103 ns |  0.7579 ns |  0.94 | 0.0331 |     - |     - |     104 B |
    |     SystemLinq |            10 |    232.77 ns |  0.5522 ns |  0.4895 ns |  1.00 | 0.0331 |     - |     - |     104 B |
    |    CisternLinq |            10 |    222.41 ns |  0.5708 ns |  0.5340 ns |  0.96 | 0.0331 |     - |     - |     104 B |
    |                |               |              |            |            |       |        |       |       |           |
    |  SystemForLoop |          1000 | 12,280.46 ns | 58.0790 ns | 51.4855 ns |  0.92 | 0.0305 |     - |     - |     104 B |
    | CisternForLoop |          1000 | 12,703.87 ns | 29.6182 ns | 27.7049 ns |  0.95 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 13,329.80 ns | 17.5620 ns | 16.4275 ns |  1.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  9,632.12 ns | 25.0778 ns | 23.4578 ns |  0.72 | 0.0305 |     - |     - |     104 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectEnumerable_Aggreate : SelectEnumerableBase
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
		public double SystemLinq() => System.Linq.Enumerable.Aggregate(SystemNumbers, 0.0, (a, c) => a + c);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Aggregate(CisternNumbers, 0.0, (a, c) => a + c);
    }
}
