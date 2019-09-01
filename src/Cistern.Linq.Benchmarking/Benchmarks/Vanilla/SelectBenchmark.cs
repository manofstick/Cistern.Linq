using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    [CoreJob, MemoryDiagnoser]
	public class SelectBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double[] ForLoop()
		{
			var items = new double[NumberOfItems];
			foreach (var n in Numbers)
			{
				items[(int)n - 1] = n + 1;
			}
			return items;
		}

		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			var items = new double[NumberOfItems];
			foreach (var item in System.Linq.Enumerable.Select(Numbers, n => n + 1))
			{
				items[(int)item - 2] = item;
			}
			return items;
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			var items = new double[NumberOfItems];
			foreach (var item in Enumerable.Select(Numbers, n => n + 1))
			{
				items[(int)item - 2] = item;
			}
			return items;
		}
	}
}
