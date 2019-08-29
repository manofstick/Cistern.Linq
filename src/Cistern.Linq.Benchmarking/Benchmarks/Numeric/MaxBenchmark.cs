using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    [CoreJob, MemoryDiagnoser]
	public class MaxBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			double max = 0;
			foreach (var n in Numbers)
			{
				if (n > max)
				{
					max = n;
				}
			}
			
			if (Numbers.Length == 0)
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			
			return max;
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
