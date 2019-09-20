using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class Range<T>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<T, int>
        , Optimizations.ICountOnConsumable
        , Optimizations.IMergeSelect<T>
        , Optimizations.IMergeWhere<T>
        , Optimizations.IMergeSkipTake<T>
    {
        private readonly int _start;
        private readonly int _count;

        public Range(int start, int count, ILink<int, T> first) : base(first) =>
            (_start, _count) = (start, count);

        public override Consumable<T> Create   (ILink<int, T> first) => new Range<T>(_start, _count, first);
        public override Consumable<U> Create<U>(ILink<int, U> first) => new Range<U>(_start, _count, first);

        public override IEnumerator<T> GetEnumerator() =>
            ChainLinq.GetEnumerator.Range.Get(_start, _count, Link);

        public override void Consume(Consumer<T> consumer) =>
            ChainLinq.Consume.Range.Invoke(_start, _count, Link, consumer);

        int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap)
        {
            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                var count = countLink.GetCount(_count);
                if (count >= 0)
                    return count;
            }

            if (onlyIfCheap)
            {
                return -1;
            }

            var counter = new Consumer.Count<T, int, int, double, Maths.OpsInt>();
            Consume(counter);
            return counter.Result;
        }

        public override object TailLink => IsIdentity ? this : base.TailLink;

        Consumable<U> Optimizations.IMergeSelect<T>.MergeSelect<U>(ConsumableCons<T> _, Func<T, U> selector) =>
            new SelectEnumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int, U>(new Consume.Range.RangeEnumerable(_start, _count), (Func<int, U>)(object)selector);

        public Consumable<T> MergeWhere(ConsumableCons<T> _, Func<T, bool> predicate) =>
            (Consumable<T>)(object)new WhereEnumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int>(new Consume.Range.RangeEnumerable(_start, _count), (Func<int, bool>) (object)predicate);

        public Consumable<T> MergeSkip(ConsumableCons<T> consumable, int skip)
        {
            checked
            {
                var start = _start + skip;
                var count = _count - skip;
                if (count <= 0)
                    return Empty<T>.Instance;
                return new Range<T>(start, count, Link);
            }
        }

        public Consumable<T> MergeTake(ConsumableCons<T> consumable, int take)
        {
            checked
            {
                var count = Math.Min(_count, take);
                if (count <= 0)
                    return Empty<T>.Instance;
                return new Range<T>(_start, count, Link);
            }
        }
    }
}
