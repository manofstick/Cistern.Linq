using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Select.List
{
    public abstract class SelectListBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public List<double> numbers;

        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(numbers, x => x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(numbers, x => x);

        [GlobalSetup]
        public void Setup()
        {
            numbers = Cistern.Linq.Enumerable.Range(1, NumberOfItems).Select(x => (double)x).ToList();
        }
    }
}
