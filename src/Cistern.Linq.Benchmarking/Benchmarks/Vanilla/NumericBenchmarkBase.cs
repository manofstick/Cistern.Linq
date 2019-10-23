using BenchmarkDotNet.Attributes;
using System;

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

            var r = new Random(42);
            for (var i=0; i< Numbers.Length; ++i)
            {
                var j = r.Next(i, Numbers.Length);
                var t = Numbers[i];
                Numbers[i] = Numbers[j];
                Numbers[j] = t;
            }
        }
    }
}
