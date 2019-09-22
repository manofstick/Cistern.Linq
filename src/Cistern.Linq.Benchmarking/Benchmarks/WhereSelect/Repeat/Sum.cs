using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.WhereSelect.Repeat
{
    /*
    */

    [CoreJob, MemoryDiagnoser]
	public class WhereSelectRepeat_Sum : WhereSelectRepeatBase
    {
        [Benchmark]
        public double SystemForLoop()
        {
            double sum = 0;
            foreach (var n in SystemNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark]
        public double CisternForLoop()
        {
            double sum = 0;
            foreach (var n in CisternNumbers)
            {
                sum += n;
            }
            return sum;
        }

        [Benchmark(Baseline = true)]
		public double SystemLinq() => System.Linq.Enumerable.Sum(SystemNumbers);
		
		[Benchmark]
		public double CisternLinq() => Cistern.Linq.Enumerable.Sum(CisternNumbers);
	}
}
