using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
	[CoreJob, MemoryDiagnoser]
	public class ToListBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public List<double> ForLoop()
		{
			var list = new List<double>(NumberOfItems);
			
			foreach (var n in Numbers)
			{
				list.Add(n);
			}

			return list;
		}

		[Benchmark(Baseline = true)]
		public List<double> SystemLinq()
		{
			return System.Linq.Enumerable.ToList(Numbers);
		}
		
		[Benchmark]
		public List<double> CisternLinq()
		{
			return Enumerable.ToList(Numbers);
		}
	}
}
