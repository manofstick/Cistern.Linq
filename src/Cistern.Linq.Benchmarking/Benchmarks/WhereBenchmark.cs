using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class WhereBenchmark
	{
		[Params(10000, 1000000)]
		public int NumberOfItems;

		public double[] Numbers;

		[GlobalSetup]
		public void Setup()
		{
			Numbers = new double[NumberOfItems];
			for (int i = 0; i < NumberOfItems; i++)
			{
				Numbers[i] = i + 1;
			}
		}

		[Benchmark]
		public double ForLoop()
		{
			foreach (var n in Numbers)
			{
				if (n == NumberOfItems)
				{
					return n;
				}
			}

			throw new Exception("Not found!");
		}

		[Benchmark(Baseline = true)]
		public double SystemLinq()
		{
			return System.Linq.Enumerable.First(System.Linq.Enumerable.Where(Numbers, n => n == NumberOfItems));
		}
		
		[Benchmark()]
		public double CisternLinq()
		{
			return Enumerable.First(Enumerable.Where(Numbers, n => n == NumberOfItems));
		}
	}
}
