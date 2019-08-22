using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class SumBenchmark
	{
		[Params(1000, 10000, 20000)]
		public int NumberOfItems;

		public double[] Numbers;

		[GlobalSetup]
		public void Setup()
		{
			Numbers = new double[NumberOfItems];
			for (int i = 0; i < NumberOfItems; i++)
			{
				Numbers[i] = i;
			}
		}

		[Benchmark]
		public double ForLoop()
		{
			double sum = 0;
			foreach (var n in Numbers)
			{
				if (n >= 5.0)
				{
					sum += n;
				}
			}
			return sum;
		}

		[Benchmark(Baseline = true)]
		public double SystemLinqSum()
		{
			return System.Linq.Enumerable.Sum(System.Linq.Enumerable.Where(Numbers, n => n >= 5.0));
		}
		
		[Benchmark()]
		public double CisternLinqSum()
		{
			return Enumerable.Sum(Enumerable.Where(Numbers, n => n >= 5.0));
		}
	}
}
