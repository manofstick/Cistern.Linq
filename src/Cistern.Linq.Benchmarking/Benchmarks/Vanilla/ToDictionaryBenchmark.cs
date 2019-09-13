using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    /*
    |      Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     17.49 ns |   0.0823 ns |   0.0770 ns |  0.44 |    0.00 | 0.0254 |     - |     - |      80 B |
    |  SystemLinq |             0 |     40.15 ns |   0.4046 ns |   0.3785 ns |  1.00 |    0.00 | 0.0254 |     - |     - |      80 B |
    | CisternLinq |             0 |     58.11 ns |   0.4757 ns |   0.4450 ns |  1.45 |    0.02 | 0.0355 |     - |     - |     112 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |             1 |     60.71 ns |   1.5829 ns |   1.7594 ns |  0.68 |    0.02 | 0.0685 |     - |     - |     216 B |
    |  SystemLinq |             1 |     90.24 ns |   0.4167 ns |   0.3694 ns |  1.00 |    0.00 | 0.0685 |     - |     - |     216 B |
    | CisternLinq |             1 |    128.89 ns |   0.6874 ns |   0.6094 ns |  1.43 |    0.01 | 0.0913 |     - |     - |     288 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |            10 |    183.47 ns |   1.4140 ns |   1.3227 ns |  0.81 |    0.01 | 0.1397 |     - |     - |     440 B |
    |  SystemLinq |            10 |    226.80 ns |   0.9311 ns |   0.7775 ns |  1.00 |    0.00 | 0.1397 |     - |     - |     440 B |
    | CisternLinq |            10 |    309.30 ns |   1.6290 ns |   1.5237 ns |  1.36 |    0.01 | 0.1626 |     - |     - |     512 B |
    |             |               |              |             |             |       |         |        |       |       |           |
    |     ForLoop |          1000 | 15,983.99 ns | 141.7047 ns | 132.5507 ns |  0.94 |    0.01 | 9.7961 |     - |     - |   31016 B |
    |  SystemLinq |          1000 | 17,014.72 ns | 111.1837 ns | 104.0013 ns |  1.00 |    0.00 | 9.7961 |     - |     - |   31016 B |
    | CisternLinq |          1000 | 17,345.37 ns | 107.0503 ns |  89.3918 ns |  1.02 |    0.01 | 9.7961 |     - |     - |   31088 B |
    */
    [CoreJob, MemoryDiagnoser]
	public class ToDictionaryBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public Dictionary<double, double> ForLoop()
		{
			var result = new Dictionary<double, double>(NumberOfItems);
			
			foreach (var n in Numbers)
			{
				result.Add(n, n);
			}

			return result;
		}
		
		[Benchmark(Baseline = true)]
		public Dictionary<double, double> SystemLinq()
		{
			return System.Linq.Enumerable.ToDictionary(Numbers, n => n);
		}
		
		[Benchmark]
		public Dictionary<double, double> CisternLinq()
		{
			return Enumerable.ToDictionary(Numbers, n => n);
		}
	}
}
