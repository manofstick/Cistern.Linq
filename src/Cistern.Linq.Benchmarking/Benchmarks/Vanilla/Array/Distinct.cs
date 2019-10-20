using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Distinct : VanillaCustomerArrayBase
    {
		[Benchmark]
		public double ForLoop() => (new HashSet<string>(States)).Count;

		[Benchmark(Baseline = true)]
		public int SystemLinq() => System.Linq.Enumerable.Distinct(States).Count();
		
		[Benchmark]
		public double CisternLinq() => System.Linq.Enumerable.Distinct(States).Count();
    }
}
