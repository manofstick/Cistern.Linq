using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cistern.Linq.Consumables
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SystemLinq_ConsumablesLookupDebugView<,>))]
    internal abstract partial class Lookup<TKey, TElement> 
        : ConsumableCons<IGrouping<TKey, TElement>>
        , ILookup<TKey, TElement>
        , Optimizations.IConsumableFastCount
    {
        GroupingArrayPool<TElement> _pool;

        protected GroupingInternal<TKey, TElement>[] _groupings;
        protected GroupingInternal<TKey, TElement> _lastGrouping;

        internal Lookup()
        {
            _groupings = new GroupingInternal<TKey, TElement>[7];
            _pool = new GroupingArrayPool<TElement>();
        }

        public int Count { get; protected set; }

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Links.Identity<object>.Instance, asCountConsumer);

        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            Count;

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                Grouping<TKey, TElement> grouping = GetGrouping(key, create: false);
                if (grouping != null)
                {
                    return grouping;
                }

                return Empty<TElement>.Instance;
            }
        }

        public bool Contains(TKey key) => GetGrouping(key, create: false) != null;

        internal ConsumableCons<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
            new LookupResultsSelector<TKey, TElement, TResult>(_lastGrouping, Count, resultSelector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override object TailLink => null;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override Consumable<IGrouping<TKey, TElement>> AddTail(ILink<IGrouping<TKey, TElement>, IGrouping<TKey, TElement>> transform) =>
            new Lookup<TKey, TElement, IGrouping<TKey, TElement>>(_lastGrouping, Count, transform);

        public override Consumable<U> AddTail<U>(ILink<IGrouping<TKey, TElement>, U> transform) =>
            new Lookup<TKey, TElement, U>(_lastGrouping, Count, transform);

        public override IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Lookup.Get(_lastGrouping, Links.Identity<IGrouping<TKey, TElement>>.Instance);

        public override void Consume(Consumer<IGrouping<TKey, TElement>> consumer) =>
            Cistern.Linq.Consume.Lookup.Invoke(_lastGrouping, Links.Identity<IGrouping<TKey,TElement>>.Instance, consumer);

        internal abstract GroupingInternal<TKey, TElement> GetGrouping(TKey key, bool create);

        private GroupingInternal<TKey, TElement>[] Resize()
        {
            int newSize = checked((Count * 2) + 1);
            GroupingInternal<TKey, TElement>[] newGroupings = new GroupingInternal<TKey, TElement>[newSize];
            GroupingInternal<TKey, TElement> g = _lastGrouping;
            do
            {
                g = g._next;
                int index = g._hashCode % newSize;
                g._hashNext = newGroupings[index];
                newGroupings[index] = g;
            }
            while (g != _lastGrouping);

            return newGroupings;
        }

        protected GroupingInternal<TKey, TElement> Create(TKey key, int hashCode)
        {
            if (Count == _groupings.Length)
            {
                _groupings = Resize();
            }

            int index = hashCode % _groupings.Length;
            GroupingInternal<TKey, TElement> g = new GroupingInternal<TKey, TElement>(_pool);
            g._key = key;
            g._hashCode = hashCode;
            g._hashNext = _groupings[index];
            _groupings[index] = g;
            if (_lastGrouping == null)
            {
                g._next = g;
            }
            else
            {
                g._next = _lastGrouping._next;
                _lastGrouping._next = g;
            }

            _lastGrouping = g;
            Count++;
            return g;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SystemLinq_ConsumablesLookupDebugView<,>))]
    internal sealed partial class LookupWithComparer<TKey, TElement> : Lookup<TKey, TElement>
    {
        private readonly IEqualityComparer<TKey> _comparer;

        internal LookupWithComparer(IEqualityComparer<TKey> comparer) =>
            _comparer = comparer;

        internal sealed override GroupingInternal<TKey, TElement> GetGrouping(TKey key, bool create)
        {
            int hashCode = (key == null) ? 0 : _comparer.GetHashCode(key) & 0x7FFFFFFF;
            GroupingInternal<TKey, TElement> g = _groupings[hashCode % _groupings.Length];
            while(true)
            {
                if (g == null)
                {
                    return create ? Create(key, hashCode) : null;
                }

                if (g._hashCode == hashCode && _comparer.Equals(g._key, key))
                {
                    return g;
                }

                g = g._hashNext;
            }
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SystemLinq_ConsumablesLookupDebugView<,>))]
    internal sealed partial class LookupDefaultComparer<TKey, TElement> : Lookup<TKey, TElement>
    {
        private readonly EqualityComparer<TKey> _comparer = EqualityComparer<TKey>.Default;

        internal sealed override GroupingInternal<TKey, TElement> GetGrouping(TKey key, bool create)
        {
            int hashCode = (key == null) ? 0 : _comparer.GetHashCode(key) & 0x7FFFFFFF;
            GroupingInternal<TKey, TElement> g = _groupings[hashCode % _groupings.Length];
            while (true)
            {
                if (g == null)
                {
                    return create ? Create(key, hashCode) : null;
                }

                if (g._hashCode == hashCode && _comparer.Equals(g._key, key))
                {
                    return g;
                }

                g = g._hashNext;
            }
        }
    }

    sealed partial class Lookup<TKey, TValue, V>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, IGrouping<TKey, TValue>>
        , Optimizations.IConsumableFastCount
    {
        private readonly Grouping<TKey, TValue> _lastGrouping;
        private readonly int _count;

        public Lookup(Grouping<TKey, TValue> lastGrouping, int count, ILink<IGrouping<TKey, TValue>, V> first) : base(first) =>
            (_lastGrouping, _count) = (lastGrouping, count);

        public override Consumable<V> Create(ILink<IGrouping<TKey, TValue>, V> first) =>
            new Lookup<TKey, TValue, V>(_lastGrouping, _count, first);
        public override Consumable<W> Create<W>(ILink<IGrouping<TKey, TValue>, W> first) =>
            new Lookup<TKey, TValue, W>(_lastGrouping, _count, first);

        public override IEnumerator<V> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Lookup.Get(_lastGrouping, Link);

        public override void Consume(Consumer<V> consumer) =>
            Cistern.Linq.Consume.Lookup.Invoke(_lastGrouping, Link, consumer);

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Link, asCountConsumer);

        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            _count;
    }

    class LookupResultsSelector<TKey, TElement, TResult>
        : ConsumableCons<TResult>
        , Optimizations.IConsumableFastCount
    {
        private readonly Grouping<TKey, TElement> _lastGrouping;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;
        private readonly int _count;

        public LookupResultsSelector(Grouping<TKey, TElement> lastGrouping, int count, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
            (_lastGrouping, _count, _resultSelector) = (lastGrouping, count, resultSelector);

        public override object TailLink => null;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override Consumable<TResult> AddTail(ILink<TResult, TResult> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, TResult>(_lastGrouping, _count, _resultSelector, first);

        public override Consumable<W> AddTail<W>(ILink<TResult, W> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, W>(_lastGrouping, _count, _resultSelector, first);

        public override IEnumerator<TResult> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Lookup.Get(_lastGrouping, _resultSelector, Links.Identity<TResult>.Instance);

        public override void Consume(Consumer<TResult> consumer) =>
            Cistern.Linq.Consume.Lookup.Invoke(_lastGrouping, _resultSelector, Links.Identity<TResult>.Instance, consumer);

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Links.Identity<object>.Instance, asCountConsumer);

        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            _count;
    }

    sealed partial class LookupResultsSelector<TKey, TElement, TResult, V>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, TResult>
        , Optimizations.IConsumableFastCount
    {
        private readonly Grouping<TKey, TElement> _lastGrouping;
        private readonly int _count;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public LookupResultsSelector(Grouping<TKey, TElement> lastGrouping, int count, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, ILink<TResult, V> first) : base(first) =>
            (_lastGrouping, _count, _resultSelector) = (lastGrouping, count, resultSelector);

        public override Consumable<V> Create(ILink<TResult, V> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, V>(_lastGrouping, _count, _resultSelector, first);
        public override Consumable<W> Create<W>(ILink<TResult, W> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, W>(_lastGrouping, _count, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Lookup.Get(_lastGrouping, _resultSelector, Link);

        public override void Consume(Consumer<V> consumer) =>
            Cistern.Linq.Consume.Lookup.Invoke(_lastGrouping, _resultSelector, Link, consumer);

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Link, asCountConsumer);
        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            _count;
    }
}
