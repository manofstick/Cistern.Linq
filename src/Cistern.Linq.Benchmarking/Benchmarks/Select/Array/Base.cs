using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Select.Array
{
    public abstract class SelectArrayBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public double[] numbers;
        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(numbers, x => x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(numbers, x => x);

        [GlobalSetup]
        public void Setup()
        {
            numbers = new double[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                numbers[i] = i + 1;
            }
        }
    }
}
