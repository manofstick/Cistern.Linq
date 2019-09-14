using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |    14.39 ns |  0.1334 ns |  0.1248 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    43.88 ns |  0.3250 ns |  0.3040 ns |  3.05 |    0.03 | 0.0152 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |             1 |    14.89 ns |  0.1451 ns |  0.1358 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             1 |    42.51 ns |  0.2919 ns |  0.2730 ns |  2.85 |    0.03 | 0.0152 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |            10 |    90.66 ns |  0.6174 ns |  0.5473 ns |  1.00 |    0.00 | 0.0355 |     - |     - |     112 B |
    | CisternLinq |            10 |   100.33 ns |  0.7537 ns |  0.7050 ns |  1.11 |    0.01 | 0.0559 |     - |     - |     176 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 2,377.05 ns | 20.7402 ns | 19.4004 ns |  1.00 |    0.00 | 1.2932 |     - |     - |    4072 B |
    | CisternLinq |          1000 |   503.09 ns |  6.0332 ns |  5.6435 ns |  0.21 |    0.00 | 1.3132 |     - |     - |    4136 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class TakeBenchmark : NumericBenchmarkBase
	{
		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Take(Numbers, NumberOfItems / 2));
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Enumerable.Take(Numbers, NumberOfItems / 2));
		}
	}
}
