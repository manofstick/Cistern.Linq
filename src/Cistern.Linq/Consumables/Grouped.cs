using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    internal sealed partial class GroupedEnumerable<TSource, TKey>
        : Consumable<IGrouping<TKey, TSource>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, bool delaySourceException)
        {
            if (!delaySourceException && source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            _source = source;
            _keySelector = keySelector;
            _comparer = comparer;
        }

        public override object TailLink => null;

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override IConsumable<IGrouping<TKey, TSource>> AddTail(ILink<IGrouping<TKey, TSource>, IGrouping<TKey, TSource>> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, IGrouping<TKey, TSource>>(_source, _keySelector, _comparer, transform);

        public override IConsumable<U> AddTail<U>(ILink<IGrouping<TKey, TSource>, U> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, U>(_source, _keySelector, _comparer, transform);

        private Lookup<TKey, TSource> ToLookup()
        {
            if (_source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return Consumer.Lookup.Consume(_source, _keySelector, _comparer);
        }

        public override void Consume(Consumer<IGrouping<TKey, TSource>> consumer) =>
            ToLookup().Consume(consumer);

        public override IEnumerator<IGrouping<TKey, TSource>> GetEnumerator() =>
            ToLookup().GetEnumerator();
    }

    internal sealed partial class GroupedEnumerableWithLinks<TSource, TKey, V>
        : Consumable<IGrouping<TKey, TSource>, V>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TSource>, V> link) : base(link) =>
            (_source, _keySelector, _comparer) = (source, keySelector, comparer);

        public override IConsumable<V> Create(ILink<IGrouping<TKey, TSource>, V> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, V>(_source, _keySelector, _comparer, first);
        public override IConsumable<W> Create<W>(ILink<IGrouping<TKey, TSource>, W> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, W>(_source, _keySelector, _comparer, first);

        private IConsumable<V> ToConsumable()
        {
            if (_source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            Lookup<TKey, TSource> lookup = Consumer.Lookup.Consume(_source, _keySelector, _comparer);
            return lookup.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedEnumerable<TSource, TKey, TElement>
        : Consumable<IGrouping<TKey, TElement>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
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

            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _comparer = comparer;
        }

        public override object TailLink => null;

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override IConsumable<IGrouping<TKey, TElement>> AddTail(ILink<IGrouping<TKey, TElement>, IGrouping<TKey, TElement>> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, IGrouping<TKey, TElement>>(_source, _keySelector, _elementSelector, _comparer, transform);

        public override IConsumable<U> AddTail<U>(ILink<IGrouping<TKey, TElement>, U> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, U>(_source, _keySelector, _elementSelector, _comparer, transform);

        private Lookup<TKey, TElement> ToLookup() =>
            Consumer.Lookup.Consume(_source, _keySelector, _elementSelector, _comparer);

        public override void Consume(Consumer<IGrouping<TKey, TElement>> consumer) =>
            ToLookup().Consume(consumer);

        public override IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() =>
            ToLookup().GetEnumerator();
    }

    internal sealed partial class GroupedEnumerableWithLinks<TSource, TKey, TElement, V>
        : Consumable<IGrouping<TKey, TElement>, V>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TElement>, V> link) : base(link) =>
            (_source, _keySelector, _elementSelector, _comparer) = (source, keySelector, elementSelector, comparer);

        public override IConsumable<V> Create(ILink<IGrouping<TKey, TElement>, V> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, V>(_source, _keySelector, _elementSelector, _comparer, first);
        public override IConsumable<W> Create<W>(ILink<IGrouping<TKey, TElement>, W> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, W>(_source, _keySelector, _elementSelector, _comparer, first);

        private IConsumable<V> ToConsumable()
        {
            Lookup<TKey, TElement> lookup = Consumer.Lookup.Consume(_source, _keySelector, _elementSelector, _comparer);
            return lookup.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TResult>
        : Consumable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;

        public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            if (resultSelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
            }

            _source = source;
            _keySelector = keySelector;
            _resultSelector = resultSelector;
            _comparer = comparer;
        }

        public override object TailLink => null;

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override IConsumable<TResult> AddTail(ILink<TResult, TResult> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, TResult>(_source, _keySelector, _resultSelector, _comparer, transform);

        public override IConsumable<U> AddTail<U>(ILink<TResult, U> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, U>(_source, _keySelector, _resultSelector, _comparer, transform);

        private Lookup<TKey, TSource> ToLookup() =>
            Consumer.Lookup.Consume(_source, _keySelector, _comparer);

        public override void Consume(Consumer<TResult> consumer) =>
            ToLookup().ApplyResultSelector(_resultSelector).Consume(consumer);

        public override IEnumerator<TResult> GetEnumerator() =>
            ToLookup().ApplyResultSelector(_resultSelector).GetEnumerator();
    }

    internal sealed partial class GroupedResultEnumerableWithLinks<TSource, TKey, TResult, V>
        : Consumable<TResult, V>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;

        public GroupedResultEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer, ILink<TResult, V> link) : base(link) =>
            (_source, _keySelector, _resultSelector, _comparer) = (source, keySelector, resultSelector, comparer);

        public override IConsumable<V> Create(ILink<TResult, V> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, V>(_source, _keySelector, _resultSelector, _comparer, first);
        public override IConsumable<W> Create<W>(ILink<TResult, W> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, W>(_source, _keySelector, _resultSelector, _comparer, first);

        private IConsumable<V> ToConsumable()
        {
            Lookup<TKey, TSource> lookup = Consumer.Lookup.Consume(_source, _keySelector, _comparer);
            IConsumable<TResult> appliedSelector = lookup.ApplyResultSelector(_resultSelector);
            return appliedSelector.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TElement, TResult>
        : Consumable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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

            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _comparer = comparer;
            _resultSelector = resultSelector;
        }

        public override object TailLink => null;

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new ArgumentException("TailLink is null, so this shouldn't be called");

        public override IConsumable<TResult> AddTail(ILink<TResult, TResult> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, TResult>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, transform);

        public override IConsumable<U> AddTail<U>(ILink<TResult, U> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, U>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, transform);

        private Lookup<TKey, TElement> ToLookup() =>
            Consumer.Lookup.Consume(_source, _keySelector, _elementSelector, _comparer);

        public override void Consume(Consumer<TResult> consumer) =>
            ToLookup().ApplyResultSelector(_resultSelector).Consume(consumer);

        public override IEnumerator<TResult> GetEnumerator() =>
            ToLookup().ApplyResultSelector(_resultSelector).GetEnumerator();
    }

    internal sealed partial class GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, V> : Consumable<TResult, V>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public GroupedResultEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer, ILink<TResult, V> link) : base(link) =>
            (_source, _keySelector, _elementSelector, _resultSelector, _comparer) = (source, keySelector, elementSelector, resultSelector, comparer);

        public override IConsumable<V> Create(ILink<TResult, V> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, V>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, first);
        public override IConsumable<W> Create<W>(ILink<TResult, W> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, W>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, first);

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
    }

}
