using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
    |  SystemLinq |             0 |  67.53 ns | 0.6532 ns | 0.6110 ns |  1.00 |    0.00 | 0.0151 |     - |     - |      48 B |
    | CisternLinq |             0 |  76.83 ns | 0.3580 ns | 0.3349 ns |  1.14 |    0.01 | 0.0228 |     - |     - |      72 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |             1 |  75.26 ns | 0.4770 ns | 0.4462 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |             1 |  82.44 ns | 0.7221 ns | 0.6754 ns |  1.10 |    0.01 | 0.0330 |     - |     - |     104 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |            10 |  72.22 ns | 0.5021 ns | 0.4697 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |            10 | 113.35 ns | 0.7976 ns | 0.7070 ns |  1.57 |    0.01 | 0.0457 |     - |     - |     144 B |
    |             |               |           |           |           |       |         |        |       |       |           |
    |  SystemLinq |          1000 |  72.54 ns | 0.4687 ns | 0.4384 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |          1000 | 113.03 ns | 0.8640 ns | 0.7659 ns |  1.56 |    0.02 | 0.0457 |     - |     - |     144 B |
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
