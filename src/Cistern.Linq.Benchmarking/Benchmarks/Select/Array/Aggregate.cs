using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    17.80 ns |   0.2664 ns |   0.2492 ns |  0.56 |    0.01 |      - |     - |     - |         - |
    |  SystemForLoop |             0 |    27.78 ns |   0.4591 ns |   0.4294 ns |  0.87 |    0.02 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    31.87 ns |   0.4707 ns |   0.4403 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    38.70 ns |   0.5940 ns |   0.5556 ns |  1.21 |    0.03 |      - |     - |     - |         - |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    35.77 ns |   0.5144 ns |   0.4812 ns |  0.59 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |             1 |    59.07 ns |   1.1516 ns |   1.0772 ns |  0.97 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |             1 |    61.05 ns |   0.5238 ns |   0.4374 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |             1 |    67.54 ns |   1.0620 ns |   0.9934 ns |  1.10 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   105.61 ns |   1.3526 ns |   1.1295 ns |  0.73 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |            10 |   136.14 ns |   2.5005 ns |   2.3389 ns |  0.94 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |            10 |   145.02 ns |   2.2636 ns |   2.1174 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |            10 |   104.60 ns |   1.5670 ns |   1.4658 ns |  0.72 |    0.02 | 0.0153 |     - |     - |      48 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 6,769.61 ns |  86.5144 ns |  80.9256 ns |  0.81 |    0.01 | 0.0153 |     - |     - |      48 B |
    |  SystemForLoop |          1000 | 6,574.26 ns |  96.3468 ns |  85.4089 ns |  0.79 |    0.02 | 0.0153 |     - |     - |      48 B |
    |     SystemLinq |          1000 | 8,347.75 ns | 120.7019 ns | 106.9991 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      48 B |
    |    CisternLinq |          1000 | 4,087.55 ns |  69.9182 ns |  65.4016 ns |  0.49 |    0.01 | 0.0153 |     - |     - |      48 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class SelectArray_Aggreate : SelectArrayBase
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
