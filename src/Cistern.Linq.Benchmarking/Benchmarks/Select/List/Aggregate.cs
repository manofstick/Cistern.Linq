using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.List
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemForLoop |             0 |     40.78 ns |  0.2862 ns |  0.2537 ns |  0.91 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             0 |     42.78 ns |  0.3968 ns |  0.3711 ns |  0.95 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             0 |     44.93 ns |  0.2777 ns |  0.2598 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             0 |     82.87 ns |  0.7494 ns |  0.7010 ns |  1.84 |    0.02 | 0.0380 |     - |     - |     120 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |             1 |     52.40 ns |  0.4669 ns |  0.4367 ns |  0.90 |    0.01 | 0.0228 |     - |     - |      72 B |
    | CisternForLoop |             1 |     54.76 ns |  0.3969 ns |  0.3712 ns |  0.94 |    0.01 | 0.0228 |     - |     - |      72 B |
    |     SystemLinq |             1 |     58.30 ns |  0.4434 ns |  0.4148 ns |  1.00 |    0.00 | 0.0228 |     - |     - |      72 B |
    |    CisternLinq |             1 |     87.71 ns |  0.7413 ns |  0.6934 ns |  1.50 |    0.02 | 0.0380 |     - |     - |     120 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |            10 |    145.82 ns |  1.3538 ns |  1.2664 ns |  0.81 |    0.01 | 0.0226 |     - |     - |      72 B |
    | CisternForLoop |            10 |    150.69 ns |  0.8535 ns |  0.7984 ns |  0.83 |    0.01 | 0.0226 |     - |     - |      72 B |
    |     SystemLinq |            10 |    180.85 ns |  1.7448 ns |  1.5467 ns |  1.00 |    0.00 | 0.0226 |     - |     - |      72 B |
    |    CisternLinq |            10 |    145.92 ns |  1.2613 ns |  1.1798 ns |  0.81 |    0.01 | 0.0379 |     - |     - |     120 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    |  SystemForLoop |          1000 |  9,497.49 ns | 58.0119 ns | 51.4261 ns |  0.85 |    0.01 | 0.0153 |     - |     - |      72 B |
    | CisternForLoop |          1000 |  9,852.75 ns | 78.4979 ns | 73.4270 ns |  0.88 |    0.01 | 0.0153 |     - |     - |      72 B |
    |     SystemLinq |          1000 | 11,232.09 ns | 65.2996 ns | 61.0813 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      72 B |
    |    CisternLinq |          1000 |  6,132.56 ns | 37.1293 ns | 28.9881 ns |  0.55 |    0.00 | 0.0305 |     - |     - |     120 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectList_Aggreate : SelectListBase
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
