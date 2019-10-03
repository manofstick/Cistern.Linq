using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed class Enumerable<TEnumerable, TEnumerator, T, U>
        : Consumable<T, U>
        , Optimizations.IMergeSelect<U>
        , Optimizations.IMergeWhere<U>
        , Optimizations.IConsumableFastCount
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }

        public Enumerable(TEnumerable enumerable, ILink<T, U> link) : base(link) => Underlying = enumerable;

        public override IConsumable<U> Create   (ILink<T, U> first) => new Enumerable<TEnumerable, TEnumerator, T, U>(Underlying, first);
        public override IConsumable<V> Create<V>(ILink<T, V> first) => new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, first);

        public override IEnumerator<U> GetEnumerator() =>
            new ConsumerEnumerators.Enumerable<TEnumerable, TEnumerator, T, U>(Underlying, Link);

        public override void Consume(Consumer<U> consumer)
        {
            if (Underlying.TryGetSourceAsSpan(out var span))
                Cistern.Linq.Consume.ReadOnlySpan.Invoke(span, Link, consumer);
            else
                Cistern.Linq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, U>(Underlying, Link, consumer);
        }

        public override object TailLink => IsIdentity ? this : base.TailLink;

        IConsumable<V> Optimizations.IMergeSelect<U>.MergeSelect<V>(IConsumable<U> _, Func<U, V> selector) =>
            new SelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, (Func<T, V>)(object)selector);

        public IConsumable<U> MergeWhere(IConsumable<U> _, Func<U, bool> predicate) =>
            (IConsumable<U>)(object)new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, (Func<T, bool>)(object)predicate);

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Link, asCountConsumer);

        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            Underlying.TryLength;
    }
}
