using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
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
