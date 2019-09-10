using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class Repeat<T, U> 
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T>
        , Optimizations.ICountOnConsumable
    {
        private readonly T _element;
        private readonly int _count;

        public Repeat(T element, int count, ILink<T, U> first) : base(first) =>
            (_element, _count) = (element, count);

        public override Consumable<U> Create   (ILink<T, U> first) => new Repeat<T, U>(_element, _count, first);
        public override Consumable<V> Create<V>(ILink<T, V> first) => new Repeat<T, V>(_element, _count, first);

        public override IEnumerator<U> GetEnumerator() =>
            ChainLinq.GetEnumerator.Repeat.Get(_element, _count, Link);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.Repeat.Invoke(_element, _count, Link, consumer);

        int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap)
        {
            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                return countLink.GetCount(_count);
            }

            if (onlyIfCheap)
            {
                return -1;
            }

            return FullCount();
        }

        private int FullCount()
        {
            var counter = new Consumer.Count<U, int, int, Maths.OpsInt>();
            Consume(counter);
            return counter.Result;
        }

    }
}
