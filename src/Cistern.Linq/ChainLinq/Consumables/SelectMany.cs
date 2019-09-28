using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class SelectMany<Enumerable, T, V>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
        , Optimizations.IConsumableFastCount
        where Enumerable : IEnumerable<T>
    {
        private readonly Consumable<Enumerable> _selectMany;

        public SelectMany(Consumable<Enumerable> enumerable, ILink<T, V> first) : base(first) =>
            _selectMany = enumerable;

        public override Consumable<V> Create   (ILink<T, V> first) => new SelectMany<Enumerable, T, V>(_selectMany, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new SelectMany<Enumerable, T, W>(_selectMany, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, Link, consumer);

        public int? TryFastCount(bool asConsumer)
        {
            // we don't care about the count in _selectMany, but we do care if we can do it as a consumer, 
            // because that is how we 
            if (_selectMany is Optimizations.IConsumableFastCount fast && fast.TryFastCount(true).HasValue)
                return Optimizations.Count.TryGetCount(this, Link, asConsumer);
            return null;
        }

        public int? TryRawCount(bool asConsumer) =>
            Utils.Consume(_selectMany, new Consumer.CountSelectMany<Enumerable, T>(asConsumer));
    }

    sealed partial class SelectMany<TSource, TCollection, T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly Consumable<(TSource, IEnumerable<TCollection>)> _selectMany;
        private readonly Func<TSource, TCollection, T> _resultSelector;

        public SelectMany(Consumable<(TSource, IEnumerable<TCollection>)> enumerable, Func<TSource, TCollection, T> resultSelector, ILink<T, V> first) : base(first) =>
            (_selectMany, _resultSelector) = (enumerable, resultSelector);

        public override Consumable<V> Create   (ILink<T, V> first) => new SelectMany<TSource, TCollection, T, V>(_selectMany, _resultSelector, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new SelectMany<TSource, TCollection, T, W>(_selectMany, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, _resultSelector, Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, _resultSelector, Link, consumer);
    }
}
