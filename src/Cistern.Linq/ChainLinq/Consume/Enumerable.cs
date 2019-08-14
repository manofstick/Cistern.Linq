using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class Enumerable
    {
        public static void Invoke<TEnumerable, TEnumerator, T, V>(TEnumerable source, Link<T, V> composition, Chain<V> consumer)
            where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
            where TEnumerator : IEnumerator<T>
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IHeadStart<T> optimized)
                {
                    optimized.Execute(source);
                }
                else
                {
                    Pipeline<TEnumerable, TEnumerator, T>(source, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<TEnumerable, TEnumerator, T>(TEnumerable source, Chain<T> chain)
            where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
            where TEnumerator : IEnumerator<T>
        {
            foreach (var item in source)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
