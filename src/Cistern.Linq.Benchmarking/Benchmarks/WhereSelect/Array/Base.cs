using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.WhereSelect.Array
{
    public abstract class WhereSelectArrayBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public double[] numbers;
        public IEnumerable<double> CisternNumbers => Cistern.Linq.Enumerable.Select(Cistern.Linq.Enumerable.Where(numbers, _ => true), x => x);
        public IEnumerable<double> SystemNumbers => System.Linq.Enumerable.Select(System.Linq.Enumerable.Where(numbers, _ => true), x => x);

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
