using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Consume
{
    static class IList
    {
        struct IListEnumerator<T>
            : IEnumerator<T>
        {
            private readonly IList<T> list;
            private readonly int completeIdx;
            int idx;

            public IListEnumerator(IList<T> list, int start, int count)
            {
                this.idx = start - 1;
                checked { completeIdx = idx + count; }
                this.list = list;
            }

            public T Current => list[idx];

            public bool MoveNext()
            {
                if (idx < completeIdx)
                {
                    ++idx;
                    return true;
                }
                return false;
            }

            public void Dispose() { }

            public void Reset() => throw new System.NotImplementedException();
            object IEnumerator.Current => throw new System.NotImplementedException();
        }

        struct IListEnumerable<T> : Optimizations.ITypedEnumerable<T, IListEnumerator<T>>
        {
            private readonly IList<T> list;
            private readonly int start;
            private readonly int count;

            public IEnumerable<T> Source => null;
            public int? TryLength => count;

            public IListEnumerable(IList<T> list, int start, int count)
            {
                this.list = list;
                this.start = start;
                this.count = count;
            }

            public IListEnumerator<T> GetEnumerator() => new IListEnumerator<T>(list, start, count);

            public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
            {
                readOnlySpan = default;
                return false;
            }

            public bool TryLast(out T result)
            {
                if (count == 0)
                {
                    result = default;
                    return false;
                }
                result = list[start + count - 1];
                return true;
            }
        }

        public static void Invoke<T>(IList<T> array, int start, int count, Chain<T> chain)
        {
            try
            {
                var status = ChainStatus.Flow;

                if (chain is Optimizations.IHeadStart<T> optimized)
                {
                    status = optimized.Execute<IListEnumerable<T>, IListEnumerator<T>>(new IListEnumerable<T>(array, start, count));
                }
                else
                {
                    status = Pipeline(array, start, count, chain);
                }

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(IList<T> array, int start, int count, ILink<T, V> composition, Chain<V> consumer) =>
            Invoke(array, start, count, composition.Compose(consumer));

        private static ChainStatus Pipeline<T>(IList<T> list, int start, int count, Chain<T> chain)
        {
            var state = ChainStatus.Flow;
            int completeIdx;
            checked { completeIdx = start + count; }
            for (var idx = start; idx < completeIdx; ++idx)
            {
                state = chain.ProcessNext(list[idx]);
                if (state.IsStopped())
                    break;
            }
            return state;
        }
    }
}
