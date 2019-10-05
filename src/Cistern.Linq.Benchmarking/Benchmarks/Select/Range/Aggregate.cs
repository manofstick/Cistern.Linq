using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |    29.79 ns |   0.2762 ns |   0.2583 ns |  1.14 |    0.02 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |    24.59 ns |   0.1989 ns |   0.1763 ns |  0.94 |    0.02 |      - |     - |     - |         - |
    |     SystemLinq |             0 |    26.11 ns |   0.5467 ns |   0.5114 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |    52.40 ns |   0.8339 ns |   0.7800 ns |  2.01 |    0.06 | 0.0076 |     - |     - |      24 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |    60.54 ns |   1.2524 ns |   1.0458 ns |  1.01 |    0.02 | 0.0254 |     - |     - |      80 B |
    |  SystemForLoop |             1 |    57.61 ns |   0.7997 ns |   0.7090 ns |  0.96 |    0.02 | 0.0280 |     - |     - |      88 B |
    |     SystemLinq |             1 |    60.01 ns |   0.9115 ns |   0.8526 ns |  1.00 |    0.00 | 0.0280 |     - |     - |      88 B |
    |    CisternLinq |             1 |    91.06 ns |   0.8891 ns |   0.8317 ns |  1.52 |    0.02 | 0.0254 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |   145.55 ns |   2.4120 ns |   1.8831 ns |  1.00 |    0.02 | 0.0253 |     - |     - |      80 B |
    |  SystemForLoop |            10 |   127.15 ns |   1.7192 ns |   1.6082 ns |  0.87 |    0.01 | 0.0279 |     - |     - |      88 B |
    |     SystemLinq |            10 |   145.51 ns |   1.9429 ns |   1.8174 ns |  1.00 |    0.00 | 0.0279 |     - |     - |      88 B |
    |    CisternLinq |            10 |   130.73 ns |   1.2293 ns |   1.1499 ns |  0.90 |    0.02 | 0.0253 |     - |     - |      80 B |
    |                |               |             |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 8,347.69 ns |  98.3104 ns |  91.9596 ns |  1.01 |    0.02 | 0.0153 |     - |     - |      80 B |
    |  SystemForLoop |          1000 | 6,959.11 ns |  86.0111 ns |  80.4548 ns |  0.84 |    0.01 | 0.0229 |     - |     - |      88 B |
    |     SystemLinq |          1000 | 8,278.83 ns | 111.8026 ns | 104.5803 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      88 B |
    |    CisternLinq |          1000 | 4,901.25 ns |  59.8557 ns |  55.9890 ns |  0.59 |    0.01 | 0.0229 |     - |     - |      80 B |
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
