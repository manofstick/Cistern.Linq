using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed class Delayed<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        internal Func<ConsumableCons<T>> GetUnderlying { get; }

        public Delayed(Func<ConsumableCons<T>> consumable, ILink<T, V> link) : base(link) =>
            GetUnderlying = consumable;

        public override Consumable<V> Create   (ILink<T, V> first) => new Delayed<T, V>(GetUnderlying, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new Delayed<T, W>(GetUnderlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            GetUnderlying().AddTail(Link).GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            GetUnderlying().AddTail(Link).Consume(consumer);
    }
}
