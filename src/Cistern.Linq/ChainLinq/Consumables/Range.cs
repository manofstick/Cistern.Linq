using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class Range<T>
        : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<T, int>
        , Optimizations.ISkipTakeOnConsumable<T>
        , Optimizations.ICountOnConsumable
    {
        private readonly int _start;
        private readonly int _count;

        public Range(int start, int count, Link<int, T> first) : base(first) =>
            (_start, _count) = (start, count);

        public override Consumable<T> Create   (Link<int, T> first) => new Range<T>(_start, _count, first);
        public override Consumable<U> Create<U>(Link<int, U> first) => new Range<U>(_start, _count, first);

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

            var counter = new Consumer.Count<T, int, int, Maths.OpsInt>();
            Consume(counter);
            return counter.Result;
        }

        T Optimizations.ISkipTakeOnConsumable<T>.Last(bool orDefault)
        {
            var skipped = ((Optimizations.ISkipTakeOnConsumable<T>)this).Skip(_count - 1);

            var last = new Consumer.Last<T>(orDefault);
            skipped.Consume(last);
            return last.Result;
        }

        Consumable<T> Optimizations.ISkipTakeOnConsumable<T>.Skip(int toSkip)
        {
            if (toSkip == 0)
                return this;

            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<int, T> skipLink)
            {
                checked
                {
                    var newCount = _count - toSkip;
                    if (newCount <= 0)
                    {
                        return Empty<T>.Instance;
                    }

                    var newStart = _start + toSkip;
                    var newLink = skipLink.Skip(toSkip);

                    return new Range<T>(newStart, newCount, newLink);
                }
            }
            return AddTail(new Links.Skip<T>(toSkip));
        }

        Consumable<T> Optimizations.ISkipTakeOnConsumable<T>.Take(int count)
        {
            if (count <= 0)
            {
                return Empty<T>.Instance;
            }

            if (count >= _count)
            {
                return this;
            }

            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<int, T>)
            {
                return new Range<T>(_start, count, Link);
            }

            return AddTail(new Links.Take<T>(count));
        }
    }
}
