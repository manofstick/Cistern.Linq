using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |  69.31 ns | 0.5875 ns | 0.5496 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |             0 |  63.08 ns | 0.6002 ns | 0.5615 ns |  0.91 |    0.01 | 0.0279 |     - |     - |      88 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |             1 |  77.16 ns | 0.5461 ns | 0.5108 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |             1 | 129.18 ns | 1.4631 ns | 1.3686 ns |  1.67 |    0.02 | 0.0455 |     - |     - |     144 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |            10 |  72.50 ns | 0.5907 ns | 0.5526 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |            10 |  98.76 ns | 0.9385 ns | 0.7327 ns |  1.36 |    0.02 | 0.0457 |     - |     - |     144 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |          1000 |  72.77 ns | 0.4594 ns | 0.4298 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |          1000 |  95.46 ns | 1.0327 ns | 0.9154 ns |  1.31 |    0.02 | 0.0457 |     - |     - |     144 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class SkipBenchmark : NumericBenchmarkBase
	{
		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Skip(Numbers, NumberOfItems - 1));
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Enumerable.Skip(Numbers, NumberOfItems - 1));
		}
	}
}
