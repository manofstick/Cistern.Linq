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

        protected int[] SortedMap(TElement[] buffer) => GetEnumerableSorter().Sort(buffer);

/*
        private int[] SortedMap(TElement[] buffer, int minIdx, int maxIdx) =>
            GetEnumerableSorter().Sort(buffer, minIdx, maxIdx);
*/

        public override IEnumerator<TElement> GetEnumerator()
        {
            var buffer = _source.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = SortedMap(buffer);
                return OrderByImpl.GetEnumerator(buffer, map);
            }
            return Empty<TElement>.Instance.GetEnumerator();
        }

#if ORDERBY_FORCE_CREATE_ARRAY
        IConsumable<TElement> Optimizations.IDelayed<TElement>.Force()
        {
            var buffer = _source.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance;

            int[] map = SortedMap(buffer);
            for (int i = 0; i < map.Length && i < buffer.Length; i++)
            {
                var j = map[i];
                while (j < i)
                    j = map[j];
                if (i == j)
                    continue;
                var tmp = buffer[j];
                buffer[j] = buffer[i];
                buffer[i] = tmp;
            }

            return new Array<TElement, TElement>(buffer, 0, buffer.Length, null);
        }
#else
        IConsumable<TElement> Optimizations.IDelayed<TElement>.Force()
        {
            var buffer = _source.ToArray();
            if (buffer.Length == 0)
                return Empty<TElement>.Instance;

            int[] map = SortedMap(buffer);

            return new Enumerable<OrderByEnumerable<TElement>, OrderByEnumerator<TElement>, TElement, TElement>(new OrderByEnumerable<TElement>(buffer, map), null);
        }

#endif
        /*
                internal IEnumerator<TElement> GetEnumerator(int minIdx, int maxIdx)
                {
                    var buffer = _source.ToArray();
                    if (buffer.Length > minIdx)
                    {
                        if (buffer.Length <= maxIdx)
                        {
                            maxIdx = buffer.Length - 1;
                        }

                        if (minIdx == maxIdx)
                        {
                            yield return GetEnumerableSorter().ElementAt(buffer, minIdx);
                        }
                        else
                        {
                            int[] map = SortedMap(buffer, minIdx, maxIdx);
                            while (minIdx <= maxIdx)
                            {
                                yield return buffer[map[minIdx]];
                                ++minIdx;
                            }
                        }
                    }
                }
        */
        System.Linq.IOrderedEnumerable<TElement> System.Linq.IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending) =>
            new OrderBy<TElement, TKey>(_source, keySelector, comparer, @descending, this);

        private EnumerableSorter<TElement> GetEnumerableSorter() => GetEnumerableSorter(null);

        internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);
/*
        private CachingComparer<TElement> GetComparer() => GetComparer(null);

        internal abstract CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer);


        public TElement TryGetFirst(Func<TElement, bool> predicate, out bool found)
        {
            CachingComparer<TElement> comparer = GetComparer();
            using IEnumerator<TElement> e = _source.GetEnumerator();
            TElement value;
            do
            {
                if (!e.MoveNext())
                {
                    found = false;
                    return default;
                }

                value = e.Current;
            }
            while (!predicate(value));

            comparer.SetElement(value);
            while (e.MoveNext())
            {
                TElement x = e.Current;
                if (predicate(x) && comparer.Compare(x, true) < 0)
                {
                    value = x;
                }
            }

            found = true;
            return value;
        }

        public TElement TryGetLast(Func<TElement, bool> predicate, out bool found)
        {
            CachingComparer<TElement> comparer = GetComparer();
            using IEnumerator<TElement> e = _source.GetEnumerator();
            TElement value;
            do
            {
                if (!e.MoveNext())
                {
                    found = false;
                    return default;
                }

                value = e.Current;
            }
            while (!predicate(value));

            comparer.SetElement(value);
            while (e.MoveNext())
            {
                TElement x = e.Current;
                if (predicate(x) && comparer.Compare(x, false) >= 0)
                {
                    value = x;
                }
            }

            found = true;
            return value;
        }
*/
    }

    internal sealed class OrderBy<TElement, TKey> : OrderBy<TElement>
    {
        private readonly OrderBy<TElement> _parent;
        private readonly Func<TElement, TKey> _keySelector;
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

        internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
        {
            EnumerableSorter<TElement> sorter = EnumerableSorter<TElement, TKey>.FactoryCreate(_keySelector, _comparer, _descending, next);
            if (_parent != null)
            {
                sorter = _parent.GetEnumerableSorter(sorter);
            }

            return sorter;
        }
        /*
                internal override CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer)
                {
                    CachingComparer<TElement> cmp = childComparer == null
                        ? new CachingComparer<TElement, TKey>(_keySelector, _comparer, _descending)
                        : new CachingComparerWithChild<TElement, TKey>(_keySelector, _comparer, _descending, childComparer);
                    return _parent != null ? _parent.GetComparer(cmp) : cmp;

                }
        */
        // TODO: Just piggy-backing on standard Enumerables here - can probably do better
        public override void Consume(Consumer<TElement> consumer)
        {
            var buffer = _source.ToArray();
            if (buffer.Length > 0)
            {
                int[] map = SortedMap(buffer);
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

        public override IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, TElement>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override IConsumable<U> AddTail<U>(ILink<TElement, U> transform) =>
            new Enumerable<Optimizations.IEnumerableEnumerable<TElement>, System.Collections.Generic.IEnumerator<TElement>, TElement, U>(new Optimizations.IEnumerableEnumerable<TElement>(this), transform);

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotSupportedException();
        public override object TailLink => null;
    }

/*
    // A comparer that chains comparisons, and pushes through the last element found to be
    // lower or higher (depending on use), so as to represent the sort of comparisons
    // done by OrderBy().ThenBy() combinations.
    internal abstract class CachingComparer<TElement>
    {
        internal abstract int Compare(TElement element, bool cacheLower);

        internal abstract void SetElement(TElement element);
    }

    internal class CachingComparer<TElement, TKey> : CachingComparer<TElement>
    {
        protected readonly Func<TElement, TKey> _keySelector;
        protected readonly IComparer<TKey> _comparer;
        protected readonly bool _descending;
        protected TKey _lastKey;

        public CachingComparer(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            _keySelector = keySelector;
            _comparer = comparer;
            _descending = descending;
        }

        internal override int Compare(TElement element, bool cacheLower)
        {
            TKey newKey = _keySelector(element);
            int cmp = _descending ? _comparer.Compare(_lastKey, newKey) : _comparer.Compare(newKey, _lastKey);
            if (cacheLower == cmp < 0)
            {
                _lastKey = newKey;
            }

            return cmp;
        }

        internal override void SetElement(TElement element)
        {
            _lastKey = _keySelector(element);
        }
    }

    internal sealed class CachingComparerWithChild<TElement, TKey> : CachingComparer<TElement, TKey>
    {
        private readonly CachingComparer<TElement> _child;

        public CachingComparerWithChild(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, CachingComparer<TElement> child)
            : base(keySelector, comparer, descending)
        {
            _child = child;
        }

        internal override int Compare(TElement element, bool cacheLower)
        {
            TKey newKey = _keySelector(element);
            int cmp = _descending ? _comparer.Compare(_lastKey, newKey) : _comparer.Compare(newKey, _lastKey);
            if (cmp == 0)
            {
                return _child.Compare(element, cacheLower);
            }

            if (cacheLower == cmp < 0)
            {
                _lastKey = newKey;
                _child.SetElement(element);
            }

            return cmp;
        }

        internal override void SetElement(TElement element)
        {
            base.SetElement(element);
            _child.SetElement(element);
        }
    }
*/

    internal abstract class EnumerableSorter<TElement>
        : IComparer<int>
    {
        internal abstract void ComputeKeys(TElement[] elements);

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
/*
        internal int[] Sort(TElement[] elements, int minIdx, int maxIdx)
        {
            int[] map = ComputeMap(elements);

            PartialQuickSort(map, 0, elements.Length - 1, minIdx, maxIdx);

            return map;
        }

        internal TElement ElementAt(TElement[] elements, int idx)
        {
            int[] map = ComputeMap(elements);
            return idx == 0 ?
                elements[Min(map, elements.Length)] :
                elements[QuickSelect(map, elements.Length - 1, idx)];
        }
*/
        protected abstract void QuickSort(int[] map, int left, int right);

/*
        // Sorts the k elements between minIdx and maxIdx without sorting all elements
        // Time complexity: O(n + k log k) best and average case. O(n^2) worse case.
        protected abstract void PartialQuickSort(int[] map, int left, int right, int minIdx, int maxIdx);

        // Finds the element that would be at idx if the collection was sorted.
        // Time complexity: O(n) best and average case. O(n^2) worse case.
        protected abstract int QuickSelect(int[] map, int right, int idx);

        protected abstract int Min(int[] map, int count);
*/
        public abstract int Compare(int x, int y);
    }

    internal sealed class AscendingEnumerableSorter<TElement, TKey>
        : EnumerableSorter<TElement, TKey>
    {
        public AscendingEnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, EnumerableSorter<TElement> next)
            : base(keySelector, comparer, next)
        {}

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

            // -c will result in a negative value for int.MinValue (-int.MinValue == int.MinValue).
            // Flipping keys earlier is more likely to trigger something strange in a comparer,
            // particularly as it comes to the sort being stable.
            return c;
        }
    }

    internal sealed class DescendingEnumerableSorter<TElement, TKey>
        : EnumerableSorter<TElement, TKey>
    {
        public DescendingEnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, EnumerableSorter<TElement> next)
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

    internal abstract class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>
    {
        protected readonly Func<TElement, TKey> _keySelector;
        protected readonly IComparer<TKey> _comparer;
        protected readonly EnumerableSorter<TElement> _next;
        protected TKey[] _keys;

        internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, EnumerableSorter<TElement> next)
        {
            _keySelector = keySelector;
            _comparer = comparer;
            _next = next;
        }

        internal override void ComputeKeys(TElement[] elements)
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

/*
        private int CompareKeys(int index1, int index2) => index1 == index2 ? 0 : Compare(index1, index2);

        // Sorts the k elements between minIdx and maxIdx without sorting all elements
        // Time complexity: O(n + k log k) best and average case. O(n^2) worse case.
        protected override void PartialQuickSort(int[] map, int left, int right, int minIdx, int maxIdx)
        {
            do
            {
                int i = left;
                int j = right;
                int x = map[i + ((j - i) >> 1)];
                do
                {
                    while (i < map.Length && CompareKeys(x, map[i]) > 0)
                    {
                        i++;
                    }

                    while (j >= 0 && CompareKeys(x, map[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        int temp = map[i];
                        map[i] = map[j];
                        map[j] = temp;
                    }

                    i++;
                    j--;
                }
                while (i <= j);

                if (minIdx >= i)
                {
                    left = i + 1;
                }
                else if (maxIdx <= j)
                {
                    right = j - 1;
                }

                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        PartialQuickSort(map, left, j, minIdx, maxIdx);
                    }

                    left = i;
                }
                else
                {
                    if (i < right)
                    {
                        PartialQuickSort(map, i, right, minIdx, maxIdx);
                    }

                    right = j;
                }
            }
            while (left < right);
        }

        // Finds the element that would be at idx if the collection was sorted.
        // Time complexity: O(n) best and average case. O(n^2) worse case.
        protected override int QuickSelect(int[] map, int right, int idx)
        {
            int left = 0;
            do
            {
                int i = left;
                int j = right;
                int x = map[i + ((j - i) >> 1)];
                do
                {
                    while (i < map.Length && CompareKeys(x, map[i]) > 0)
                    {
                        i++;
                    }

                    while (j >= 0 && CompareKeys(x, map[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        int temp = map[i];
                        map[i] = map[j];
                        map[j] = temp;
                    }

                    i++;
                    j--;
                }
                while (i <= j);

                if (i <= idx)
                {
                    left = i + 1;
                }
                else
                {
                    right = j - 1;
                }

                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        right = j;
                    }

                    left = i;
                }
                else
                {
                    if (i < right)
                    {
                        left = i;
                    }

                    right = j;
                }
            }
            while (left < right);

            return map[idx];
        }

        protected override int Min(int[] map, int count)
        {
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                if (CompareKeys(map[i], map[index]) < 0)
                {
                    index = i;
                }
            }
            return map[index];
        }
*/
        internal static EnumerableSorter<TElement> FactoryCreate(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next) =>
            descending
            ? (EnumerableSorter<TElement>)new DescendingEnumerableSorter<TElement, TKey>(keySelector, comparer, next)
            : (EnumerableSorter<TElement>)new AscendingEnumerableSorter<TElement, TKey>(keySelector, comparer, next);
    }
}
