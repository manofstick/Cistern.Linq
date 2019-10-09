using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    internal sealed partial class GroupedEnumerable<TSource, TKey, TElement, V>
        : Consumable<IGrouping<TKey, TElement>, V>
    {
        private readonly bool _delaySourceException;
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TElement>, V> link, bool delaySourceException) : base(link)
        {
            if (!delaySourceException && source == null)
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

            (_delaySourceException, _source, _keySelector, _elementSelector, _comparer) = (delaySourceException, source, keySelector, elementSelector, comparer);
        }

        public override IConsumable<V> Create(ILink<IGrouping<TKey, TElement>, V> first) =>
            new GroupedEnumerable<TSource, TKey, TElement, V>(_source, _keySelector, _elementSelector, _comparer, first, _delaySourceException);

        public override IConsumable<W> Create<W>(ILink<IGrouping<TKey, TElement>, W> first) =>
            new GroupedEnumerable<TSource, TKey, TElement, W>(_source, _keySelector, _elementSelector, _comparer, first, _delaySourceException);

        private IConsumable<V> ToConsumable()
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
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TElement, TResult, V>
        : Consumable<TResult, V>
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
    }
}
