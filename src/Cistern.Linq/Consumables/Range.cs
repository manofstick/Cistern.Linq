using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed partial class Range
        : Consumable<int>
        , Optimizations.IConsumableFastCount
        , Optimizations.IMergeSelect<int>
        , Optimizations.IMergeWhere<int>
        , Optimizations.IMergeSkipTake<int>
    {
        private readonly int _start;
        private readonly int _count;
        public Range(int start, int count) =>
            (_start, _count) = (start, count);
        public override IConsumable<int> AddTail(ILink<int, int> transform) =>
            new Enumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int, int>(new Consume.Range.RangeEnumerable(_start, _count), transform);
        public override IConsumable<U> AddTail<U>(ILink<int, U> transform) =>
            new Enumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int, U>(new Consume.Range.RangeEnumerable(_start, _count), transform);
        public override IEnumerator<int> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Range.Get(_start, _count, Links.Identity<int>.Instance);
        public override void Consume(Consumer<int> consumer) =>
            Cistern.Linq.Consume.Range.Invoke(_start, _count, consumer);

        public int? TryFastCount(bool asCountConsumer) => _count;
        public int? TryRawCount(bool asCountConsumer) => _count;

        public override object TailLink => this;
        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> transform) =>
            new Enumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int, V>(new Consume.Range.RangeEnumerable(_start, _count), (ILink<int, V>)(object)transform);
        IConsumable<U> Optimizations.IMergeSelect<int>.MergeSelect<U>(Consumable<int> _, Func<int, U> selector) =>
            new SelectEnumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int, U>(new Consume.Range.RangeEnumerable(_start, _count), selector);
        IConsumable<int> Optimizations.IMergeWhere<int>.MergeWhere(Consumable<int> _, Func<int, bool> predicate) =>
            new WhereEnumerable<Consume.Range.RangeEnumerable, Consume.Range.RangeEnumerator, int>(new Consume.Range.RangeEnumerable(_start, _count), predicate);
        IConsumable<int> Optimizations.IMergeSkipTake<int>.MergeSkip(Consumable<int> consumable, int skip)
        {
            checked
            {
                var start = _start + skip;
                var count = _count - skip;
                if (count <= 0)
                    return Empty<int>.Instance;
                return new Range(start, count);
            }
        }
        IConsumable<int> Optimizations.IMergeSkipTake<int>.MergeTake(Consumable<int> consumable, int take)
        {
            checked
            {
                var count = Math.Min(_count, take);
                if (count <= 0)
                    return Empty<int>.Instance;
                return new Range(_start, count);
            }
        }
    }
}
