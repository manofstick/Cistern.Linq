using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class MaxBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			double? max = 0;
			foreach (var n in Numbers)
			{
				if (n > max || max == null)
				{
					max = n;
				}
			}
			
			if (!max.HasValue)
			{
				throw new InvalidOperationException("Sequence has no elements!");
			}
			
			return max.Value;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			return System.Linq.Enumerable.Max(Numbers);
		}
		
		[Benchmark]
		public double CisternLinq()
		{
			return Enumerable.Max(Numbers);
		}
	}
}
