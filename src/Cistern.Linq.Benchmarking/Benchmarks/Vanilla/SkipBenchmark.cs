using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |     Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |---------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 | 60.23 ns | 0.5928 ns | 0.5545 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |             0 | 49.69 ns | 0.7153 ns | 0.6341 ns |  0.82 |    0.01 | 0.0152 |     - |     - |      48 B |
    |             |               |          |           |           |       |         |        |       |       |           |
    |  SystemLinq |             1 | 66.82 ns | 0.9298 ns | 0.8697 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |             1 | 62.06 ns | 0.6909 ns | 0.6462 ns |  0.93 |    0.01 | 0.0330 |     - |     - |     104 B |
    |             |               |          |           |           |       |         |        |       |       |           |
    |  SystemLinq |            10 | 63.99 ns | 1.0912 ns | 1.0207 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |            10 | 59.80 ns | 0.7550 ns | 0.7062 ns |  0.93 |    0.01 | 0.0330 |     - |     - |     104 B |
    |             |               |          |           |           |       |         |        |       |       |           |
    |  SystemLinq |          1000 | 64.14 ns | 0.9465 ns | 0.7903 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |          1000 | 60.24 ns | 0.9686 ns | 0.9061 ns |  0.94 |    0.02 | 0.0330 |     - |     - |     104 B |
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
