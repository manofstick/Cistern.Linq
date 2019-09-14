using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class List<T, V>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
        , Optimizations.ICountOnConsumable
    {
        internal List<T> Underlying { get; }

        public List(List<T> array, ILink<T, V> first) : base(first) =>
            Underlying = array;

        public override Consumable<V> Create   (ILink<T, V> first) => new List<T, V>(Underlying, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new List<T, W>(Underlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            new ConsumerEnumerators.Enumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T, V>(new Optimizations.ListEnumerable<T>(Underlying), Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.List.Invoke(Underlying, Link, consumer);

        int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, Link, Underlying.Count, onlyIfCheap);
    }
}
