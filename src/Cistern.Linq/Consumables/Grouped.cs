using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq.Consumables
{
    // Grouping is a publically exposed class, so we provide this class get the Consumable
    [DebuggerDisplay("Key = {Key}")]
    [DebuggerTypeProxy(typeof(SystemLinq_GroupingDebugView<,>))]
    internal class GroupingInternal<TKey, TElement>
        : Grouping<TKey, TElement>
        , IConsumable<TElement>
    {
        internal GroupingInternal(Lookup<TKey, TElement> owner) : base(owner) { }

        public IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) =>
            (_count == 1)
            ? (IConsumable<TElement>)new IList<TElement, TElement>(this, 0, 1, transform)
            : (IConsumable<TElement>)new Array<TElement, TElement>(_elementsOrNull, 0, _count, transform);

        public IConsumable<U> AddTail<U>(ILink<TElement, U> transform) =>
            (_count == 1)
            ? (IConsumable<U>)new IList<TElement, U>(this, 0, 1, transform)
            : (IConsumable<U>)new Array<TElement, U>(_elementsOrNull, 0, _count, transform);

        public void Consume(Consumer<TElement> consumer)
        {
            if (_count == 1)
            {
                Cistern.Linq.Consume.IList.Invoke(this, 0, 1, consumer);
            }
            else
            {
                Cistern.Linq.Consume.ReadOnlySpan.Invoke(new ReadOnlySpan<TElement>(_elementsOrNull, 0, _count), consumer);
            }
        }
    }

    internal partial class GroupedEnumerable<TSource, TKey, TElement, V>
        : Consumable<IGrouping<TKey, TElement>, V>
        , Optimizations.IDelayed<V>
    {
        protected readonly IEnumerable<TSource> _source;
        protected readonly Func<TSource, TKey> _keySelector;
        protected readonly IEqualityComparer<TKey> _comparer;
        protected readonly bool _delaySourceException;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly bool _noElementSelector;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TElement>, V> link, bool delaySourceException)
            : this(source, keySelector, elementSelector, false, comparer, link, delaySourceException) { }
        
        protected GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, bool noElementSelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TElement>, V> link, bool delaySourceException) : base(link)
        {
            if (!delaySourceException && source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            if (!noElementSelector && elementSelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
            }

            (_noElementSelector, _delaySourceException, _source, _keySelector, _elementSelector, _comparer) =
            ( noElementSelector,  delaySourceException,  source,  keySelector,  elementSelector,  comparer);
        }

        public override IConsumable<V> Create(ILink<IGrouping<TKey, TElement>, V> first) =>
            new GroupedEnumerable<TSource, TKey, TElement, V>(_source, _keySelector, _elementSelector, _noElementSelector, _comparer, first, _delaySourceException);

        public override IConsumable<W> Create<W>(ILink<IGrouping<TKey, TElement>, W> first) =>
            new GroupedEnumerable<TSource, TKey, TElement, W>(_source, _keySelector, _elementSelector, _noElementSelector, _comparer, first, _delaySourceException);

        protected virtual IConsumable<V> ToConsumable()
        {
            if (_source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

            var lookup = Consumer.Lookup.Consume(_source, _keySelector, _elementSelector, _comparer);
            
            return IsIdentity ? (IConsumable<V>)lookup : lookup.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            ToConsumable().Consume(consumer);

        public IConsumable<V> Force() => ToConsumable();
    }

    class GroupedEnumerable<TSource, TKey, V>
        : GroupedEnumerable<TSource, TKey, TSource, V>
    {
        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TSource>, V> link, bool delaySourceException)
            : base(source, keySelector, null, true, comparer, link, delaySourceException)
        {}

        public override IConsumable<V> Create(ILink<IGrouping<TKey, TSource>, V> first) =>
            new GroupedEnumerable<TSource, TKey, V>(_source, _keySelector, _comparer, first, _delaySourceException);

        public override IConsumable<W> Create<W>(ILink<IGrouping<TKey, TSource>, W> first) =>
            new GroupedEnumerable<TSource, TKey, W>(_source, _keySelector, _comparer, first, _delaySourceException);


        protected override IConsumable<V> ToConsumable()
        {
            if (_source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

            var lookup = Consumer.Lookup.Consume(_source, _keySelector, _comparer);

            return lookup.AddTail(Link);
        }
    }
    class GroupedEnumerable<TSource, TKey>
        : GroupedEnumerable<TSource, TKey, IGrouping<TKey, TSource>>
    {
        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, bool delaySourceException)
            : base(source, keySelector, comparer, null, delaySourceException)
        {}

        protected override IConsumable<IGrouping<TKey, TSource>> ToConsumable()
        {
            if (_source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

            return Consumer.Lookup.Consume(_source, _keySelector, _comparer);
        }
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TElement, TResult, V>
        : Consumable<TResult, V>
        , Optimizations.IDelayed<V>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer, ILink<TResult, V> link) : base(link)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            if (elementSelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
            }

            if (resultSelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
            }

            (_source, _keySelector, _elementSelector, _resultSelector, _comparer) = (source, keySelector, elementSelector, resultSelector, comparer);
        }

        public override IConsumable<V> Create(ILink<TResult, V> first) =>
            new GroupedResultEnumerable<TSource, TKey, TElement, TResult, V>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, first);
        public override IConsumable<W> Create<W>(ILink<TResult, W> first) =>
            new GroupedResultEnumerable<TSource, TKey, TElement, TResult, W>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, first);

        private IConsumable<V> ToConsumable()
        {
            Lookup<TKey, TElement> lookup = Consumer.Lookup.Consume(_source, _keySelector, _elementSelector, _comparer);
            IConsumable<TResult> appliedSelector = lookup.ApplyResultSelector(_resultSelector);
            return appliedSelector.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            ToConsumable().Consume(consumer);

        public IConsumable<V> Force() => ToConsumable();
    }
}
