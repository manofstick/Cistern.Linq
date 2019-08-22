using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class SumBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double ForLoop()
		{
			double sum = 0;
			for (int i = 0; i < NumberOfItems; i++)
			{
				sum += Numbers[i];
			}
			return sum;
		}

		[Benchmark]
		public double ForEachLoop()
		{
			double sum = 0;
			foreach (var n in Numbers)
			{
				sum += n;
			}
			return sum;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			return System.Linq.Enumerable.Sum(Numbers);
		}
		
		[Benchmark]
		public double CisternLinq()
		{
			return Enumerable.Sum(Numbers);
		}
	}
}
