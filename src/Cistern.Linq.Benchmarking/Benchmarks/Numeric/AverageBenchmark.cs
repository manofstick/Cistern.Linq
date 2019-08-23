using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
	[CoreJob, MemoryDiagnoser]
	public class AverageBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			var count = 0;
			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
				count++;
			}
			
			if (count == 0)
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}

			return sum / count;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			return System.Linq.Enumerable.Average(Numbers);
		}
		
		[Benchmark]
		public double CisternLinq()
		{
			return Enumerable.Average(Numbers);
		}
	}
}
