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

    abstract class OrderByForced<TElement>
        : IConsumable<TElement>
    {
        private readonly TElement[] buffer;
        private readonly int[] map;

        public OrderByForced(TElement[] buffer, int[] map) =>
            (this.buffer, this.map) = (buffer, map);

        public IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) => throw new NotSupportedException();

        public IConsumable<U> AddTail<U>(ILink<TElement, U> transform) => throw new NotSupportedException();

        public void Consume(Consumer<TElement> consumer)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TElement> GetEnumerator() => OrderByImpl.GetEnumerator(buffer, map);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    abstract class OrderBy<TElement>
        : Consumable<TElement>
        , System.Linq.IOrderedEnumerable<TElement>
        , Optimizations.IDelayed<TElement>
    {
        internal IEnumerable<TElement> _source;

        public override IEnumerator<TElement> GetEnumerator() => GetEnumerableSorter().GetEnumerator(_source);

        IConsumable<TElement> Optimizations.IDelayed<TElement>.Force() => GetEnumerableSorter().Force(_source);

        protected virtual IEnumerableSorter<TElement> GetEnumerableSorter() => GetEnumerableSorter(null);

        internal abstract IEnumerableSorter<TElement> GetEnumerableSorter(IEnumerableSorter<TElement> next);

        public abstract System.Linq.IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);

        public override object TailLink => null;
        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotSupportedException();
    }

    internal abstract class OrderByCommon<TElement, TKey> : OrderBy<TElement>
    {
        protected readonly OrderBy<TElement> _parent;
        protected readonly Func<TElement, TKey> _keySelector;

        internal OrderByCommon(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, OrderBy<TElement> parent)
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
        }

        public override void Consume(Consumer<TElement> consumer) =>
            GetEnumerableSorter().Consume(_source, consumer);

        // TODO: Just piggy-backing on standard Enumerables here - can probably do better
        public override IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, TElement>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override IConsumable<U> AddTail<U>(ILink<TElement, U> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, U>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override System.Linq.IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey2>(Func<TElement, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        {
            return new OrderBy<TElement, TKey2>(_source, keySelector, comparer, @descending, this);
        }
    }

    abstract class IndexSorter<TElement>
    {
        public abstract void IndexSort(TElement[] data, int[] indexes, int startIdx, int count);
        public abstract void Initialize(int size);
    }

    class IndexSorterTail<TElement>
        : IndexSorter<TElement>
    {
        public static IndexSorter<TElement> Instance { get; } = new IndexSorterTail<TElement>();

        private IndexSorterTail() { }

        public override void IndexSort(TElement[] data, int[] indexes, int startIdx, int count) => Array.Sort(indexes, startIdx, count);
        public override void Initialize(int size) {}
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
            IndexSort(data, indexes, 0, size);
            return indexes;
        }

        public override void IndexSort(TElement[] data, int[] indexes, int startIdx, int count)
        {
            var exclusiveEndIdx = startIdx + count;

            // copy the keys that we need
            for (var idx = startIdx; idx < exclusiveEndIdx; ++idx)
            {
                keys[idx] = keySelector(data[idx]);
                indexes[idx] = idx;
            }

            // unstable sort
            Array.Sort(keys, indexes, comparer); 

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
    }

    internal class OrderBySimple<TElement, TKey>
        : OrderByCommon<TElement, TKey>
        , IEnumerableSorter<TElement>
    {
        internal OrderBySimple(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, OrderBy<TElement> parent)
            : base(source, keySelector, parent)
        { }

        internal override IEnumerableSorter<TElement> GetEnumerableSorter(IEnumerableSorter<TElement> next)
        {
            IEnumerableSorter<TElement> sorter =
                EnumerableSorter<TElement, TKey>.FactoryCreate(_keySelector, Comparer<TKey>.Default, false, next);
            if (_parent != null)
            {
                sorter = _parent.GetEnumerableSorter(sorter);
            }
            return sorter;
        }

        public override System.Linq.IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey2>(Func<TElement, TKey2> keySelector, IComparer<TKey2> comparer, bool descending)
        {
/*
            if (!descending && (comparer == null || ReferenceEquals(comparer, Comparer<TKey2>.Default)))
                return new ThenBySimple<TElement, TKey, TKey2>(_source, _keySelector, keySelector, this);
*/
            return new OrderBy<TElement, TKey2>(_source, keySelector, comparer, @descending, this);
        }

        protected override IEnumerableSorter<TElement> GetEnumerableSorter() => this;

        internal int[] Sort(TElement[] data)
        {
            var tail = IndexSorterTail<TElement>.Instance;
            var sorter = new IndexSorterKeyed<TElement, TKey>(_keySelector, Comparer<TKey>.Default, tail);
            return sorter.StableSortedIndexes(data);
        }

        // TODO: These implementations are duplicates, so clean...

        public IEnumerator<TElement> GetEnumerator(IEnumerable<TElement> elements)
        {
            var buffer = elements.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = Sort(buffer);
                return OrderByImpl.GetEnumerator(buffer, map);
            }
            return Empty<TElement>.Instance.GetEnumerator();
        }

        public IConsumable<TElement> Force(IEnumerable<TElement> elements)
        {
            var buffer = elements.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance;

            int[] map = Sort(buffer);

            return new Enumerable<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement, TElement>(new OrderByEnumerable<TElement>(buffer, map), null);
        }

        public void Consume(IEnumerable<TElement> elements, Consumer<TElement> consumer)
        {
            var buffer = elements.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = Sort(buffer);
                Linq.Consume.Enumerable.Invoke<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement>(new OrderByEnumerable<TElement>(buffer, map), consumer);
            }
            else
            {
                try { consumer.ChainComplete(ChainStatus.Filter); }
                finally { consumer.ChainDispose(); }
            }
        }
        public int Compare(int x, int y) => throw new NotSupportedException();
        public void ComputeKeys(TElement[] elements) => throw new NotSupportedException();
    }
/*
    internal class ThenBySimple<TElement, TKey1, TKey2>
        : OrderBySimple<TElement, TKey1>
    {
        Func<TElement, TKey2> _key2Selector;

        internal ThenBySimple(IEnumerable<TElement> source, Func<TElement, TKey1> key1Selector, Func<TElement, TKey2> key2Selector, OrderBy<TElement> parent)
            : base(source, key1Selector, parent)
        {
            _key2Selector = key2Selector;
        }

        protected override TElement[] GetSortedElements(IEnumerable<TElement> elements)
        {
            var data = elements.ToArray();
            var key = new (TKey1, TKey2, int)[data.Length];
            for (var i = 0; i < data.Length && i < key.Length; ++i)
                key[i] = (_keySelector(data[i]), _key2Selector(data[i]), i);
            Array.Sort(key, data);
            return data;
        }

        // can possibly go deeper?
        public override System.Linq.IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey3>(Func<TElement, TKey3> keySelector, IComparer<TKey3> comparer, bool descending) =>
            new OrderBy<TElement, TKey3>(_source, keySelector, comparer, @descending, this);
    }
*/

    internal class OrderBy<TElement, TKey> 
        : OrderByCommon<TElement, TKey>
    {
        private IComparer<TKey> _comparer;
        private readonly bool _descending;

        internal OrderBy(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, OrderBy<TElement> parent)
            : base(source, keySelector, parent)
        {
            _comparer = comparer;
            _descending = descending;
        }

        internal override IEnumerableSorter<TElement> GetEnumerableSorter(IEnumerableSorter<TElement> next)
        {
            IEnumerableSorter<TElement> sorter = EnumerableSorter<TElement, TKey>.FactoryCreate(_keySelector, _comparer, _descending, next);
            if (_parent != null)
            {
                sorter = _parent.GetEnumerableSorter(sorter);
            }

            return sorter;
        }
    }

    interface IEnumerableSorter<TElement>
    {
        IEnumerator<TElement> GetEnumerator(IEnumerable<TElement> elements);
        IConsumable<TElement> Force(IEnumerable<TElement> elements);
        void Consume(IEnumerable<TElement> elements, Consumer<TElement> consumer);

        int Compare(int x, int y);
        void ComputeKeys(TElement[] elements);
    }


    internal abstract class EnumerableSorter<TElement>
        : IComparer<int>
        , IEnumerableSorter<TElement>
    {
        public abstract void ComputeKeys(TElement[] elements);

        private int[] ComputeMap(TElement[] elements)
        {
            ComputeKeys(elements);

            int[] map = new int[elements.Length];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = i;
            }

            return map;
        }

        internal int[] Sort(TElement[] elements)
        {
            int[] map = ComputeMap(elements);

            QuickSort(map, 0, elements.Length - 1);

            return map;
        }

        protected abstract void QuickSort(int[] map, int left, int right);

        public abstract int Compare(int x, int y);

        public IEnumerator<TElement> GetEnumerator(IEnumerable<TElement> elements)
        {
            var buffer = elements.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = Sort(buffer);
                return OrderByImpl.GetEnumerator(buffer, map);
            }
            return Empty<TElement>.Instance.GetEnumerator();
        }

        public IConsumable<TElement> Force(IEnumerable<TElement> elements)
        {
            var buffer = elements.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance;

            int[] map = Sort(buffer);

            return new Enumerable<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement, TElement>(new OrderByEnumerable<TElement>(buffer, map), null);
        }

        public void Consume(IEnumerable<TElement> elements, Consumer<TElement> consumer)
        {
            var buffer = elements.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = Sort(buffer);
                Linq.Consume.Enumerable.Invoke<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement>(new OrderByEnumerable<TElement>(buffer, map), consumer);
            }
            else
            {
                try
                {
                    consumer.ChainComplete(ChainStatus.Filter);
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
        }
    }

    internal abstract class EnumerableSorter<TElement, TKey>
        : EnumerableSorter<TElement>
    {
        protected readonly Func<TElement, TKey> _keySelector;
        protected readonly IComparer<TKey> _comparer;
        protected readonly IEnumerableSorter<TElement> _next;
        protected TKey[] _keys;

        internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, IEnumerableSorter<TElement> next)
        {
            _keySelector = keySelector;
            _comparer = comparer;
            _next = next;
        }

        public override void ComputeKeys(TElement[] elements)
        {
            _keys = new TKey[elements.Length];
            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] = _keySelector(elements[i]);
            }

            _next?.ComputeKeys(elements);
        }

        protected override void QuickSort(int[] keys, int lo, int hi) =>
            Array.Sort(keys, lo, hi - lo + 1, this);

        internal static EnumerableSorter<TElement> FactoryCreate(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, IEnumerableSorter<TElement> next)
        {
            var isDefault = comparer == null || comparer == Comparer<TKey>.Default;
            if (descending)
            {
                if (isDefault)
                    return new DescendingEnumerableDefaultSorter(keySelector, next);
                return new DescendingEnumerableSorter(keySelector, comparer, next);
            }
            if (isDefault)
                return new AscendingEnumerableDefaultSorter(keySelector, next);
            return new AscendingEnumerableSorter(keySelector, comparer, next);
        }

        sealed class AscendingEnumerableDefaultSorter
            : EnumerableSorter<TElement, TKey>
        {
            private readonly Comparer<TKey> Comparer = Comparer<TKey>.Default;

            public AscendingEnumerableDefaultSorter(Func<TElement, TKey> keySelector, IEnumerableSorter<TElement> next)
                : base(keySelector, Comparer<TKey>.Default, next)
            { }

            public override int Compare(int index1, int index2)
            {
                int c = Comparer.Compare(_keys[index1], _keys[index2]);
                if (c == 0)
                {
                    if (_next == null)
                    {
                        return index1 - index2; // ensure stability of sort
                    }

                    return _next.Compare(index1, index2);
                }

                return c;
            }
        }

        sealed class AscendingEnumerableSorter
            : EnumerableSorter<TElement, TKey>
        {
            public AscendingEnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, IEnumerableSorter<TElement> next)
                : base(keySelector, comparer, next)
            { }

            public override int Compare(int index1, int index2)
            {
                int c = _comparer.Compare(_keys[index1], _keys[index2]);
                if (c == 0)
                {
                    if (_next == null)
                    {
                        return index1 - index2; // ensure stability of sort
                    }

                    return _next.Compare(index1, index2);
                }

                return c;
            }
        }

        sealed class DescendingEnumerableDefaultSorter
            : EnumerableSorter<TElement, TKey>
        {
            private readonly Comparer<TKey> Comparer = Comparer<TKey>.Default;

            public DescendingEnumerableDefaultSorter(Func<TElement, TKey> keySelector, IEnumerableSorter<TElement> next)
                : base(keySelector, Comparer<TKey>.Default, next)
            { }

            public override int Compare(int index1, int index2)
            {
                int c = Comparer.Compare(_keys[index1], _keys[index2]);

                if (c < 0) return 1;
                if (c > 0) return -1;

                if (_next == null)
                {
                    return index1 - index2; // ensure stability of sort
                }

                return _next.Compare(index1, index2);
            }
        }

        sealed class DescendingEnumerableSorter
            : EnumerableSorter<TElement, TKey>
        {
            public DescendingEnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, IEnumerableSorter<TElement> next)
                : base(keySelector, comparer, next)
            { }

            public override int Compare(int index1, int index2)
            {
                int c = _comparer.Compare(_keys[index1], _keys[index2]);

                if (c < 0) return 1;
                if (c > 0) return -1;

                if (_next == null)
                {
                    return index1 - index2; // ensure stability of sort
                }

                return _next.Compare(index1, index2);
            }
        }
    }
}
