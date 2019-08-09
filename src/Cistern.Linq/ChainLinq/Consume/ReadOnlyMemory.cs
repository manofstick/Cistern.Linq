using System;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class ReadOnlyMemory
    {
        public static void Invoke<T, V>(ReadOnlyMemory<T> array, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IHeadStart<T> optimized)
                {
                    optimized.Execute(array.Span);
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

        internal static void Pipeline<T>(ReadOnlyMemory<T> memory, Chain<T> chain)
        {
            foreach (var item in memory.Span)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
