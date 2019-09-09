using System;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class ReadOnlySpan
    {
        const int MinSizeToCoverExecuteCosts = 10; // from some random testing (depends on pipeline length)

        public static void Invoke<T>(ReadOnlySpan<T> array, Chain<T> chain)
        {
            try
            {
                if (array.Length > MinSizeToCoverExecuteCosts && chain is Optimizations.IHeadStart<T> optimized)
                {
                    optimized.Execute(array);
                }
                else
                {
                    Pipeline(array, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(ReadOnlySpan<T> array, Link<T, V> composition, Chain<V> consumer) =>
            Invoke(array, composition.Compose(consumer));

        internal static void Pipeline<T>(ReadOnlySpan<T> memory, Chain<T> chain)
        {
            foreach (var item in memory)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
