using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed class Enumerable<TEnumerable, TEnumerator, T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }

        public Enumerable(TEnumerable enumerable, ILink<T, V> link) : base(link) => Underlying = enumerable;

        public override Consumable<V> Create   (ILink<T, V> first) => new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new Enumerable<TEnumerable, TEnumerator, T, W>(Underlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            new ConsumerEnumerators.Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, Link);

        public override void Consume(Consumer<V> consumer)
        {
            if (Underlying.TryGetSourceAsSpan(out var span))
                ChainLinq.Consume.ReadOnlySpan.Invoke(span, Link, consumer);
            else
                ChainLinq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, V>(Underlying, Link, consumer);
        }
    }
}
