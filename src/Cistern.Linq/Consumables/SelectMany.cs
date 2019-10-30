using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed partial class SelectMany<Enumerable, T, V>
        : Consumable<T, V>
        , Optimizations.IConsumableFastCount
        where Enumerable : IEnumerable<T>
    {
        private readonly IConsumable<Enumerable> _selectMany;

        public SelectMany(IConsumable<Enumerable> enumerable, ILink<T, V> first) : base(first) =>
            _selectMany = enumerable;

        public override IConsumable<V> Create   (ILink<T, V> first) => new SelectMany<Enumerable, T, V>(_selectMany, first);
        public override IConsumable<W> Create<W>(ILink<T, W> first) => new SelectMany<Enumerable, T, W>(_selectMany, first);

        public override IEnumerator<V> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.SelectMany.Get(_selectMany, Link);

        public override void Consume(Consumer<V> consumer) =>
            Cistern.Linq.Consume.SelectMany.Invoke(_selectMany, Link, consumer);

        public int? TryFastCount(bool asCountConsumer)
        {
            // we don't care about the count in _selectMany, but if asCountConsumer is false, then we need to ensure
            // that we can Consume it without causing side effects
            if (asCountConsumer || (_selectMany is Optimizations.IConsumableFastCount fast && fast.TryFastCount(true).HasValue))
                return Optimizations.Count.TryGetCount(this, LinkOrNull, asCountConsumer);
            return null;
        }

        public int? TryRawCount(bool asCountConsumer) =>
            Utils.Consume(_selectMany, new Consumer.CountSelectMany<Enumerable, T>(asCountConsumer));
    }

    sealed partial class SelectMany<TSource, TCollection, T, V> : Consumable<T, V>
    {
        private readonly IConsumable<(TSource, IEnumerable<TCollection>)> _selectMany;
        private readonly Func<TSource, TCollection, T> _resultSelector;

        public SelectMany(IConsumable<(TSource, IEnumerable<TCollection>)> enumerable, Func<TSource, TCollection, T> resultSelector, ILink<T, V> first) : base(first) =>
            (_selectMany, _resultSelector) = (enumerable, resultSelector);

        public override IConsumable<V> Create   (ILink<T, V> first) => new SelectMany<TSource, TCollection, T, V>(_selectMany, _resultSelector, first);
        public override IConsumable<W> Create<W>(ILink<T, W> first) => new SelectMany<TSource, TCollection, T, W>(_selectMany, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.SelectMany.Get(_selectMany, _resultSelector, Link);

        public override void Consume(Consumer<V> consumer) =>
            Cistern.Linq.Consume.SelectMany.Invoke(_selectMany, _resultSelector, Link, consumer);
    }
}
