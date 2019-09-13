using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |------------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |    13.72 ns |  0.1074 ns |  0.0952 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             0 |    65.98 ns |  0.4224 ns |  0.3952 ns |  4.81 |    0.05 | 0.0151 |     - |     - |      48 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |             1 |    14.36 ns |  0.1196 ns |  0.1118 ns |  1.00 |    0.00 |      - |     - |     - |         - |
    | CisternLinq |             1 |    73.95 ns |  0.4548 ns |  0.4254 ns |  5.15 |    0.05 | 0.0279 |     - |     - |      88 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |            10 |    89.90 ns |  0.4542 ns |  0.3793 ns |  1.00 |    0.00 | 0.0355 |     - |     - |     112 B |
    | CisternLinq |            10 |   127.70 ns |  0.9624 ns |  0.9002 ns |  1.42 |    0.01 | 0.0558 |     - |     - |     176 B |
    |             |               |             |            |            |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 2,400.22 ns | 12.4302 ns |  9.7047 ns |  1.00 |    0.00 | 1.2932 |     - |     - |    4072 B |
    | CisternLinq |          1000 | 2,649.56 ns | 22.7457 ns | 21.2763 ns |  1.10 |    0.01 | 1.3123 |     - |     - |    4136 B |
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
