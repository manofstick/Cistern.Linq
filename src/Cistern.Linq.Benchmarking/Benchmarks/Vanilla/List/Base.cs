using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Vanilla.List
{
    public abstract class VanillaListBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public List<double> Numbers;

        [GlobalSetup]
        public void Setup()
        {
            Numbers = Cistern.Linq.Enumerable.Range(1, NumberOfItems).Select(x => (double)x).ToList();
        }
    }
}
