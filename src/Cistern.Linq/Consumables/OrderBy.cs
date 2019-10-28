using Cistern.Linq.UtilsTmp;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    static class OrderByImpl
    {
        public static IEnumerator<TElement> GetEnumerator<TElement>(TElement[] buffer, int[] map)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                yield return buffer[map[i]];
            }
        }

        public static int[] Sort<TElement, TKey>(TElement[] data, Func<TElement, TKey> _keySelector, IComparer<TKey> _comparer, bool _descending, OrderBy<TElement> _parent, IndexSorter<TElement> tail)
        {
            var comparer = _descending ? new DescendingComparer<TKey>(_comparer) : _comparer;
            var sorter = new IndexSorterKeyed<TElement, TKey>(_keySelector, comparer, tail);
            return _parent != null ? _parent.Sort(data, sorter) : sorter.StableSortedIndexes(data);
        }
    }

    internal class DescendingComparer<TKey>
        : IComparer<TKey>
    {
        private readonly IComparer<TKey> _comparer;

        public DescendingComparer(IComparer<TKey> comparer) => _comparer = comparer;

        public int Compare(TKey x, TKey y) => _comparer.Compare(y, x);
    }

    struct OrderByEnumerator<TElement>
        : IEnumerator<TElement>
    {
        private readonly TElement[] buffer;
        private readonly int[] map;

        int index;

        public OrderByEnumerator(TElement[] buffer, int[] map) =>
            (this.buffer, this.map, this.index) = (buffer, map, -1);

        public TElement Current => buffer[map[index]];

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (++index < buffer.Length)
            {
                return true;
            }
            --index;
            return false;
        }

        public void Reset() => throw new NotSupportedException();
    }

    struct OrderByEnumerable<TElement>
        : Optimizations.ITypedEnumerable<TElement, OrderByEnumerator<TElement>>
    {
        private readonly TElement[] buffer;
        private readonly int[] map;

        public OrderByEnumerable(TElement[] buffer, int[] map) =>
            (this.buffer, this.map) = (buffer, map);

        public IEnumerable<TElement> Source => null;

        public int? TryLength => buffer.Length;

        public OrderByEnumerator<TElement> GetEnumerator() => new OrderByEnumerator<TElement>(buffer, map);

        public bool TryGetSourceAsSpan(out ReadOnlySpan<TElement> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TryLast(out TElement result)
        {
            result = default;
            return false;
        }
    }

    abstract class OrderBy<TElement>
        : Consumable<TElement>
        , System.Linq.IOrderedEnumerable<TElement>
        , Optimizations.IDelayed<TElement>
    {
        internal IEnumerable<TElement> _source;

        public override object TailLink => null;
        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotSupportedException();

        public abstract int[] Sort(TElement[] data, IndexSorter<TElement> sorter);
        
        public abstract IConsumable<TElement> Force(); // Optimizations.IDelayed<TElement>

        public System.Linq.IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending) =>
            new OrderBy<TElement, TKey>(_source, keySelector, comparer, descending, this);
    }

    internal class OrderBy<TElement, TKey>
        : OrderBy<TElement>
    {
        protected readonly OrderBy<TElement> _parent;
        protected readonly Func<TElement, TKey> _keySelector;
        private readonly IComparer<TKey> _comparer;
        private readonly bool _descending;

        internal OrderBy(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, OrderBy<TElement> parent)
        {
            if (source is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }
            if (keySelector is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            _source = source;
            _parent = parent;
            _keySelector = keySelector;
            _comparer = comparer ?? Comparer<TKey>.Default;
            _descending = descending;
        }

        public override void Consume(Consumer<TElement> consumer)
        {
            var buffer = _source.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = Sort(buffer, IndexSorterTail<TElement>.Instance);
                Linq.Consume.Enumerable.Invoke<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement>(new OrderByEnumerable<TElement>(buffer, map), consumer);
            }
            else
            {
                try { consumer.ChainComplete(ChainStatus.Filter); }
                finally { consumer.ChainDispose(); }
            }
        }

        // TODO: Just piggy-backing on standard Enumerables here - can probably do better
        public override IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, TElement>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override IConsumable<U> AddTail<U>(ILink<TElement, U> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, U>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override int[] Sort(TElement[] data, IndexSorter<TElement> tail) =>
            OrderByImpl.Sort(data, _keySelector, _comparer, _descending, _parent, tail);

        public override IEnumerator<TElement> GetEnumerator()
        {
            var buffer = _source.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance.GetEnumerator();

            int[] map = Sort(buffer, IndexSorterTail<TElement>.Instance);

            return OrderByImpl.GetEnumerator(buffer, map);
        }

        public override IConsumable<TElement> Force()
        {
            var buffer = _source.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance;

            int[] map = Sort(buffer, IndexSorterTail<TElement>.Instance);

            return new Enumerable<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement, TElement>(new OrderByEnumerable<TElement>(buffer, map), null);
        }
    }

    abstract class IndexSorter<TElement>
        : IComparer<int>
    {
        public abstract void IndexSort(TElement[] data, int[] indexes, int startIdx, int count);
        public abstract void Initialize(int size);
        public virtual void InitializeKeys(TElement[] data) { }

        public abstract int Compare(int index1, int index2); // IComparer<int>
    }

    class IndexSorterTail<TElement>
        : IndexSorter<TElement>
    {
        public static IndexSorter<TElement> Instance { get; } = new IndexSorterTail<TElement>();

        private IndexSorterTail() { }

        public override void IndexSort(TElement[] data, int[] indexes, int startIdx, int count) => Array.Sort(indexes, startIdx, count);
        public override void Initialize(int size) { }

        public override int Compare(int lhs, int rhs) { checked { return lhs - rhs; } }
    }

    abstract class IndexSorterChain<TElement> : IndexSorter<TElement>
    {
        protected IndexSorter<TElement> lower;
        public IndexSorterChain(IndexSorter<TElement> lower) => this.lower = lower;
    }

    class IndexSorterKeyed<TElement, TKey>
        : IndexSorterChain<TElement>
    {
        readonly IComparer<TKey> comparer;
        readonly Func<TElement, TKey> keySelector;
        
        TKey[] keys;

        public override void Initialize(int size)
        {
            keys = new TKey[size];
            lower.Initialize(size);
        }

        public IndexSorterKeyed(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, IndexSorter<TElement> lower)
            : base(lower)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public int[] StableSortedIndexes(TElement[] data)
        {
            var size = data.Length;
            Initialize(size);
            var indexes = new int[size];
            for (var idx = 0; idx < indexes.Length; ++idx)
                indexes[idx] = idx;

            if (size >= 1000 || FastTypeInfo<TKey>.IsValueType)
                return LayeredSort(data, indexes);

            return CombinedComparerSort(data, indexes);
        }

        private int[] CombinedComparerSort(TElement[] data, int[] indexes)
        {
            InitializeKeys(data);
            Array.Sort(indexes, this);
            return indexes;
        }

        private int[] LayeredSort(TElement[] data, int[] indexes)
        {
            IndexSort(data, indexes, 0, data.Length);
            return indexes;
        }

        public override void IndexSort(TElement[] data, int[] indexes, int startIdx, int count)
        {
            var exclusiveEndIdx = startIdx + count;

            // copy the keys that we need
            for (var idx = startIdx; idx < exclusiveEndIdx; ++idx)
            {
                keys[idx] = keySelector(data[indexes[idx]]);
            }

            // unstable sort
            Array.Sort(keys, indexes, startIdx, count, comparer); 

            // now find duplicate keys, and go to the lower level to sort
            var (examplar, examplarIdx) = (keys[startIdx], startIdx);

            int batchCount;
            for (var idx = startIdx+1; idx < exclusiveEndIdx; ++idx)
            {
                if (comparer.Compare(examplar, keys[idx]) != 0)
                {
                    batchCount = idx - examplarIdx;
                    if (batchCount > 1)
                    {
                        lower.IndexSort(data, indexes, examplarIdx, batchCount);
                    }
                    (examplar, examplarIdx) = (keys[idx], idx);
                }
            }

            // handle the remainders
            batchCount = exclusiveEndIdx - examplarIdx;
            if (batchCount > 1)
            {
                lower.IndexSort(data, indexes, examplarIdx, batchCount);
            }
        }

        public override void InitializeKeys(TElement[] data)
        {
            for (var idx=0; idx < keys.Length; ++idx)
                keys[idx] = keySelector(data[idx]);

            lower.InitializeKeys(data);
        }

        public override int Compare(int x, int y) =>
            comparer.Compare(keys[x], keys[y]) switch
            {
                0 => lower.Compare(x, y),
                var c => c
            };
    }
}
