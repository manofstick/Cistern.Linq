using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consume
{
    static class Lookup
    {
        public static void Invoke<TKey, TElement>(Grouping<TKey, TElement> lastGrouping, Chain<IGrouping<TKey, TElement>> chain)
        {
            try
            {
                var status = Pipeline(lastGrouping, chain);

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TKey, TElement, TResult>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Chain<TResult> chain)
        {
            try
            {
                var status = Pipeline(lastGrouping, resultSelector, chain);

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TKey, TElement, V>(Grouping<TKey, TElement> lastGrouping, ILink<IGrouping<TKey, TElement>, V> composition, Chain<V> consumer) =>
            Invoke(lastGrouping, composition.Compose(consumer));

        public static void Invoke<TKey, TElement, TResult, V>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, ILink<TResult, V> composition, Chain<V> consumer) =>
            Invoke(lastGrouping, resultSelector, composition.Compose(consumer));

        private static ChainStatus Pipeline<TKey, TElement>(Grouping<TKey, TElement> lastGrouping, Chain<IGrouping<TKey, TElement>> chain)
        {
            var state = ChainStatus.Flow;
            Grouping<TKey, TElement> g = lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    state = chain.ProcessNext(g);
                    if (state.IsStopped())
                        break;
                }
                while (g != lastGrouping);
            }
            return state;
        }

        private static ChainStatus Pipeline<TKey, TElement, TResult>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Chain<TResult> chain)
        {
            var state = ChainStatus.Flow;
            Grouping<TKey, TElement> g = lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    state = chain.ProcessNext(resultSelector(g.Key, g.GetEfficientList(true)));
                    if (state.IsStopped())
                        break;
                }
                while (g != lastGrouping);
            }
            return state;
        }
    }
}
