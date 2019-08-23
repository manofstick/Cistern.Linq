using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
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
