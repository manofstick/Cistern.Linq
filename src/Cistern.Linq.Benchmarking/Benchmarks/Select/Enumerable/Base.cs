using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Select.Enumerable
{
    public abstract class SelectEnumerableBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public IEnumerable<double> numbers
        {
            get
            {
                for (int i = 0; i < NumberOfItems; i++)
                {
                    yield return i + 1;
                }
            }
        }

        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(numbers, x => x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(numbers, x => x);

        [GlobalSetup]
        public void Setup()
        {
        }
    }
}
