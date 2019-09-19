using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed class Enumerable<TEnumerable, TEnumerator, T, U>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T>
        , Optimizations.IMergeSelect<U>
        , Optimizations.IMergeWhere<U>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }

        public Enumerable(TEnumerable enumerable, ILink<T, U> link) : base(link) => Underlying = enumerable;

        public override Consumable<U> Create   (ILink<T, U> first) => new Enumerable<TEnumerable, TEnumerator, T, U>(Underlying, first);
        public override Consumable<V> Create<V>(ILink<T, V> first) => new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, first);

        public override IEnumerator<U> GetEnumerator() =>
            new ConsumerEnumerators.Enumerable<TEnumerable, TEnumerator, T, U>(Underlying, Link);

        public override void Consume(Consumer<U> consumer)
        {
            if (Underlying.TryGetSourceAsSpan(out var span))
                ChainLinq.Consume.ReadOnlySpan.Invoke(span, Link, consumer);
            else
                ChainLinq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, U>(Underlying, Link, consumer);
        }

        public override object TailLink => IsIdentity ? this : base.TailLink;

        Consumable<V> Optimizations.IMergeSelect<U>.MergeSelect<V>(ConsumableCons<U> consumable, Func<U, V> selector) =>
            new SelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, (Func<T, V>)(object)selector);

        public Consumable<U> MergeWhere(ConsumableCons<U> consumable, Func<U, bool> predicate) =>
            (Consumable<U>)(object)new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, (Func<T, bool>)(object)predicate);

    }
}
