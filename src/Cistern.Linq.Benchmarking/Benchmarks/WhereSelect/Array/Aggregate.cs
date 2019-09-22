using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    /*
    |         Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |--------------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    | CisternForLoop |             0 |     41.62 ns |  0.3300 ns |  0.3087 ns |  1.07 |    0.01 | 0.0076 |     - |     - |      24 B |
    |  SystemForLoop |             0 |     36.84 ns |  0.2439 ns |  0.2281 ns |  0.94 |    0.01 |      - |     - |     - |         - |
    |     SystemLinq |             0 |     39.03 ns |  0.2310 ns |  0.2160 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    |    CisternLinq |             0 |     67.22 ns |  0.6243 ns |  0.5534 ns |  1.72 |    0.02 | 0.0228 |     - |     - |      72 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    | CisternForLoop |             1 |     76.87 ns |  0.6431 ns |  0.6015 ns |  1.12 |    0.01 | 0.0380 |     - |     - |     120 B |
    |  SystemForLoop |             1 |     65.59 ns |  0.5267 ns |  0.4927 ns |  0.96 |    0.01 | 0.0330 |     - |     - |     104 B |
    |     SystemLinq |             1 |     68.40 ns |  0.6305 ns |  0.5897 ns |  1.00 |    0.00 | 0.0330 |     - |     - |     104 B |
    |    CisternLinq |             1 |    110.33 ns |  0.3639 ns |  0.3226 ns |  1.61 |    0.02 | 0.0533 |     - |     - |     168 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    | CisternForLoop |            10 |    170.07 ns |  1.2344 ns |  1.1547 ns |  0.96 |    0.01 | 0.0379 |     - |     - |     120 B |
    |  SystemForLoop |            10 |    165.12 ns |  1.0962 ns |  0.9718 ns |  0.93 |    0.01 | 0.0329 |     - |     - |     104 B |
    |     SystemLinq |            10 |    177.78 ns |  1.1114 ns |  1.0396 ns |  1.00 |    0.00 | 0.0329 |     - |     - |     104 B |
    |    CisternLinq |            10 |    162.82 ns |  2.0309 ns |  1.8003 ns |  0.92 |    0.01 | 0.0532 |     - |     - |     168 B |
    |                |               |              |            |            |       |         |        |       |       |           |
    | CisternForLoop |          1000 |  8,347.81 ns | 62.2505 ns | 58.2292 ns |  0.82 |    0.01 | 0.0305 |     - |     - |     120 B |
    |  SystemForLoop |          1000 |  8,917.55 ns | 58.3027 ns | 54.5364 ns |  0.88 |    0.01 | 0.0305 |     - |     - |     104 B |
    |     SystemLinq |          1000 | 10,137.33 ns | 82.4756 ns | 68.8708 ns |  1.00 |    0.00 | 0.0305 |     - |     - |     104 B |
    |    CisternLinq |          1000 |  5,532.20 ns | 27.1631 ns | 25.4084 ns |  0.55 |    0.00 | 0.0458 |     - |     - |     168 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectArray_Aggreate : WhereSelectArrayBase
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
