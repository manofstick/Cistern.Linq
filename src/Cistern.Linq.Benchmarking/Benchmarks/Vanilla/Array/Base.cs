using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    public abstract class VanillaArrayBase
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
