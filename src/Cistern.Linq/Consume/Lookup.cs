using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Consume
{
    static class Lookup
    {
        public struct LookupEnumerator<TKey, TElement> : IEnumerator<IGrouping<TKey, TElement>>
        {
            private readonly Grouping<TKey, TElement> LastGroup;
            private Grouping<TKey, TElement> CurrentGroup;
            private bool finished;

            public LookupEnumerator(Grouping<TKey, TElement> lastGroup)
            {
                LastGroup = lastGroup;
                CurrentGroup = lastGroup;
                finished = false;
            }

            public IGrouping<TKey, TElement> Current => CurrentGroup;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (finished)
                    return false;

                CurrentGroup = CurrentGroup._next;
                finished = CurrentGroup == LastGroup;
                return true;
            }

            public void Reset() => throw new System.NotImplementedException();
        }

        public struct LookupEnumerable<TKey, TElement>
            : Optimizations.ITypedEnumerable<IGrouping<TKey, TElement>, LookupEnumerator<TKey, TElement>>
        {
            private readonly Grouping<TKey, TElement> LastGroup;
            private readonly int Count;

            public LookupEnumerable(Grouping<TKey, TElement> lastGroup, int count)
            {
                LastGroup = lastGroup;
                Count = count;
            }

            public IEnumerable<IGrouping<TKey, TElement>> Source => null;

            public int? TryLength => Count;

            public LookupEnumerator<TKey, TElement> GetEnumerator() => new LookupEnumerator<TKey, TElement>(LastGroup);

            public bool TryGetSourceAsSpan(out ReadOnlySpan<IGrouping<TKey, TElement>> readOnlySpan)
            {
                readOnlySpan = default;
                return false;
            }

            public bool TryLast(out IGrouping<TKey, TElement> result)
            {
                result = default;
                return false;
            }
        }

        public static void Invoke<TKey, TElement>(Grouping<TKey, TElement> lastGrouping, int count, Chain<IGrouping<TKey, TElement>> chain)
        {
            try
            {
                ChainStatus status = ChainStatus.Flow;

                if (lastGrouping != null)
                {
                    if (chain is Optimizations.IHeadStart<IGrouping<TKey, TElement>> optimized)
                        status = optimized.Execute<LookupEnumerable<TKey, TElement>, LookupEnumerator<TKey, TElement>>(new LookupEnumerable<TKey, TElement>(lastGrouping, count));
                    else
                        status = Pipeline(lastGrouping, chain);
                }

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

        public static void Invoke<TKey, TElement, V>(Grouping<TKey, TElement> lastGrouping, int count, ILink<IGrouping<TKey, TElement>, V> composition, Chain<V> consumer) =>
            Invoke(lastGrouping, count, composition.Compose(consumer));

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
