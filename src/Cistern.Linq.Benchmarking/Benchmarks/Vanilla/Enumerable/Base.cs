using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Vanilla.Enumerable
{
    public abstract class VanillaEnumerableBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public IEnumerable<double> Numbers
        {
            get
            {
                for (int i = 0; i < NumberOfItems; i++)
                {
                    yield return i + 1;
                }
            }
        }

        [GlobalSetup]
        public void Setup()
        {
        }
    }
}
