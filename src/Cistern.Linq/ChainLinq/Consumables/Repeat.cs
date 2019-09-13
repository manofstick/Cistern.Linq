using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class Repeat<T, U> 
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T>
        , Optimizations.ICountOnConsumable
        , Optimizations.IMergeSelect<U>
        , Optimizations.IMergeWhere<U>
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
            var counter = new Consumer.Count<U, int, int, double, Maths.OpsInt>();
            Consume(counter);
            return counter.Result;
        }

        public override object TailLink => IsIdentity ? this : base.TailLink;

        Consumable<V> Optimizations.IMergeSelect<U>.MergeSelect<V>(ConsumableCons<U> consumable, Func<U, V> selector) =>
            (Consumable<V>)(object)new SelectEnumerable<Consume.Repeat.RepeatEnumerable<T>, Consume.Repeat.RepeatEnumerator<T>, T, V>(new Consume.Repeat.RepeatEnumerable<T>(_element, _count), (Func<T, V>)(object)selector);

        public Consumable<U> MergeWhere(ConsumableCons<U> consumable, Func<U, bool> predicate) =>
            (Consumable<U>)(object)new WhereEnumerable<Consume.Repeat.RepeatEnumerable<T>, Consume.Repeat.RepeatEnumerator<T>, T>(new Consume.Repeat.RepeatEnumerable<T>(_element, _count), (Func<T, bool>)(object)predicate);
    }
}
