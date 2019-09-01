using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.Numeric
{
    public abstract class NumericBenchmarkBase
    {
        [Params(0, 1, 10, 1000)]
        public int NumberOfItems;

        public double[] Numbers;

        [GlobalSetup]
        public void Setup()
        {
            Numbers = new double[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                Numbers[i] = i + 1;
            }
        }
    }
}
