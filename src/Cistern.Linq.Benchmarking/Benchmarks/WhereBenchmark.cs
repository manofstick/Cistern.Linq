using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	[CoreJob, MemoryDiagnoser]
	public class WhereBenchmark : NumericBenchmarkBase
	{
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
		
		[Benchmark]
		public double CisternLinq()
		{
			return Enumerable.First(Enumerable.Where(Numbers, n => n == NumberOfItems));
		}
	}
}
