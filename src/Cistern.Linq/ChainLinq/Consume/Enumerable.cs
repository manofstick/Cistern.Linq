using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class Enumerable
    {
        public static void Invoke<TEnumerable, TEnumerator, T>(TEnumerable source, Chain<T> chain)
            where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
            where TEnumerator : IEnumerator<T>
        {
            try
            {
                ChainStatus status;

                if (chain is Optimizations.IHeadStart<T> optimized)
                {
                    status = optimized.Execute<TEnumerable, TEnumerator>(source);
                }
                else
                {
                    status = Pipeline<TEnumerable, TEnumerator, T>(source, chain);
                }

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TEnumerable, TEnumerator, T, V>(TEnumerable source, ILink<T, V> composition, Chain<V> consumer)
            where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
            where TEnumerator : IEnumerator<T>
            => Invoke<TEnumerable, TEnumerator, T>(source, composition.Compose(consumer));

        private static ChainStatus Pipeline<TEnumerable, TEnumerator, T>(TEnumerable source, Chain<T> chain)
            where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
            where TEnumerator : IEnumerator<T>
        {
            var state = ChainStatus.Flow;
            foreach (var item in source)
            {
                state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
            return state;
        }

    }
}
