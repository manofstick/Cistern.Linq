using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class List
    {
        public static void Invoke<T>(List<T> list, Chain<T> chain)
        {
            try
            {
                if (chain is Optimizations.IHeadStart<T> optimized)
                {
                    optimized.Execute<Optimizations.ListEnumerable<T>, List<T>.Enumerator>(new Optimizations.ListEnumerable<T>(list));
                }
                else
                {
                    Pipeline(list, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(List<T> list, Link<T, V> composition, Chain<V> consumer) =>
            Invoke(list, composition.Compose(consumer));

        private static void Pipeline<T>(List<T> list, Chain<T> chain)
        {
            foreach (var item in list)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
