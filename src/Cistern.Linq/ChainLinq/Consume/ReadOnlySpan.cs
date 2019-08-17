using System;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class ReadOnlySpan
    {
        public static void Invoke<T, V>(ReadOnlySpan<T> array, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IHeadStart<T> optimized)
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
