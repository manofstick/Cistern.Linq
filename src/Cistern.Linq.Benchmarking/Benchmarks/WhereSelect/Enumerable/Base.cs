using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.WhereSelect.Enumerable
{
    public abstract class WhereSelectEnumerableBase
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

        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(Cistern.Linq.Enumerable.Where(numbers, _ => true), x => x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(System.Linq.Enumerable.Where(numbers, _ => true), x => x);

        [GlobalSetup]
        public void Setup()
        {
        }
    }
}
