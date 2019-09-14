using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class IList<T, V>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
        , Optimizations.ICountOnConsumable
    {
        private readonly IList<T> _list;
        private readonly int _start;
        private readonly int _count;

        public IList(IList<T> list, int start, int count, ILink<T, V> first) : base(first) =>
            (_list, _start, _count) = (list, start, count);

        public override Consumable<V> Create   (ILink<T, V> first) => new IList<T, V>(_list, _start, _count, first);
        public override Consumable<W> Create<W>(ILink<T, W> first) => new IList<T, W>(_list, _start, _count, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.IList.Get(_list, _start, _count, Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.IList.Invoke(_list, _start, _count, Link, consumer);

        int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, Link, _count, onlyIfCheap);
    }
}
