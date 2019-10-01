using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed class Delayed<T, V> : Consumable<T, V>
    {
        internal Func<Consumable<T>> GetUnderlying { get; }

        public Delayed(Func<Consumable<T>> consumable, ILink<T, V> link) : base(link) =>
            GetUnderlying = consumable;

        public override Consumable<V> Create   (ILink<T, V> first) => new Delayed<T, V>(GetUnderlying, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new Delayed<T, W>(GetUnderlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            GetUnderlying().AddTail(Link).GetEnumerator();

        public override void Consume(Consumer<V> consumer) =>
            GetUnderlying().AddTail(Link).Consume(consumer);
    }
}
