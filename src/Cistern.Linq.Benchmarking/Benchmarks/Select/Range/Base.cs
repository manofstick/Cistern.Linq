using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Select.Range
{
    public abstract class SelectRangeBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public IEnumerable<double> numbers;
        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(Cistern.Linq.Enumerable.Range(0, NumberOfItems), x => (double)x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, NumberOfItems), x => (double)x);

        [GlobalSetup]
        public void Setup() { }
    }
}
