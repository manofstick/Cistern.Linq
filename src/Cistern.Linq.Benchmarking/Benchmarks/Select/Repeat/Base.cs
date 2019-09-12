using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Select.Repeat
{
    public abstract class SelectRepeatBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public IEnumerable<double> numbers;
        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(Cistern.Linq.Enumerable.Repeat(42, NumberOfItems), x => (double)x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(System.Linq.Enumerable.Repeat(42, NumberOfItems), x => (double)x);

        [GlobalSetup]
        public void Setup() { }
    }
}
