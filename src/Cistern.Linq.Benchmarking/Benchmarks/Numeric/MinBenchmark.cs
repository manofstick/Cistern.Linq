using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    [CoreJob, MemoryDiagnoser]
	public class MinBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			double min = 0;
			foreach (var n in Numbers)
			{
				if (n < min)
				{
					min = n;
				}
                else if (double.IsNaN(n))
                {
                    min = double.NaN;
                    break;
                }
			}
			
			if (Numbers.Length == 0)
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			
			return min;
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
