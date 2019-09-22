using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.WhereSelect.Range
{
    public abstract class WhereSelectRangeBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public IEnumerable<double> numbers;
        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(Cistern.Linq.Enumerable.Where(Cistern.Linq.Enumerable.Range(0, NumberOfItems), _ => true), x => (double)x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(System.Linq.Enumerable.Where(System.Linq.Enumerable.Range(0, NumberOfItems), _ => true), x => (double)x);

        [GlobalSetup]
        public void Setup() { }
    }
}
