using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class MinBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			double? min = 0;
			foreach (var n in Numbers)
			{
				if (n < min || min == null)
				{
					min = n;
				}
			}
			
			if (!min.HasValue)
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			
			return min.Value;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			return System.Linq.Enumerable.Min(Numbers);
		}
		
		[Benchmark]
		public double CisternLinq()
		{
			return Enumerable.Min(Numbers);
		}
	}
}
