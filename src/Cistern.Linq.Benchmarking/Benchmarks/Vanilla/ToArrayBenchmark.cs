using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    [CoreJob, MemoryDiagnoser]
	public class ToArrayBenchmark : NumericBenchmarkBase
	{
		[Benchmark]
		public double[] ForLoop()
		{
            var array = new double[Numbers.Length];
            Numbers.CopyTo(array, 0);
			return array;
		}

		[Benchmark(Baseline = true)]
		public double[] SystemLinq()
		{
			return System.Linq.Enumerable.ToArray(Numbers);
		}
		
		[Benchmark]
		public double[] CisternLinq()
		{
			return Enumerable.ToArray(Numbers);
		}
	}
}
