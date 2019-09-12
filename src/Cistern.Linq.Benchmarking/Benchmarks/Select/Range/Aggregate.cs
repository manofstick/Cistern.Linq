using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    /*
    |         Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     38.57 ns |   0.4232 ns |   0.3534 ns |  1.20 |    0.02 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     29.32 ns |   0.3768 ns |   0.3524 ns |  0.91 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     32.24 ns |   0.3069 ns |   0.2871 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     65.73 ns |   0.8497 ns |   0.7948 ns |  2.04 |    0.02 | 0.0228 |     - |     - |      72 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |             1 |     68.32 ns |   0.8640 ns |   0.8082 ns |  0.80 |    0.01 | 0.0279 |     - |     - |      88 B |
    |  SystemForLoop |             1 |     81.62 ns |   1.0765 ns |   1.0070 ns |  0.95 |    0.01 | 0.0304 |     - |     - |      96 B |
    |     SystemLinq |             1 |     85.67 ns |   0.9940 ns |   0.9298 ns |  1.00 |    0.00 | 0.0304 |     - |     - |      96 B |
    |    CisternLinq |             1 |    114.37 ns |   1.4870 ns |   1.3909 ns |  1.34 |    0.02 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |            10 |    153.53 ns |   1.9803 ns |   1.8524 ns |  0.71 |    0.01 | 0.0279 |     - |     - |      88 B |
    |  SystemForLoop |            10 |    208.12 ns |   2.1801 ns |   2.0392 ns |  0.97 |    0.01 | 0.0303 |     - |     - |      96 B |
    |     SystemLinq |            10 |    215.49 ns |   0.5644 ns |   0.5279 ns |  1.00 |    0.00 | 0.0303 |     - |     - |      96 B |
    |    CisternLinq |            10 |    182.40 ns |   3.6545 ns |   9.2353 ns |  0.80 |    0.07 | 0.0432 |     - |     - |     136 B |
    |                |               |              |             |             |       |         |        |       |       |           |
    | CisternForLoop |          1000 | 10,206.92 ns | 197.8772 ns | 194.3419 ns |  0.68 |    0.01 | 0.0153 |     - |     - |      88 B |
    |  SystemForLoop |          1000 | 14,802.67 ns | 129.9553 ns | 121.5603 ns |  0.98 |    0.02 | 0.0153 |     - |     - |      96 B |
    |     SystemLinq |          1000 | 15,104.90 ns | 197.7063 ns | 184.9346 ns |  1.00 |    0.00 | 0.0153 |     - |     - |      96 B |
    |    CisternLinq |          1000 |  5,798.32 ns |  46.3695 ns |  43.3741 ns |  0.38 |    0.01 | 0.0381 |     - |     - |     136 B |
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
