using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed partial class Array<T, V>
        : Consumable<T, V>
        , Optimizations.IConsumableFastCount
    {
        internal T[] Underlying { get; }

        private readonly int _start;
        private readonly int _length;

        public Array(T[] array, int start, int length, ILink<T, V> first) : base(first) =>
            (Underlying, _start, _length) = (array, start, length);

        public override IConsumable<V> Create   (ILink<T, V> first) => new Array<T, V>(Underlying, _start, _length, first);
        public override IConsumable<W> Create<W>(ILink<T, W> first) => new Array<T, W>(Underlying, _start, _length, first);

        public override IEnumerator<V> GetEnumerator() =>
            Cistern.Linq.GetEnumerator.Array.Get(Underlying, _start, _length, Link);

        public override void Consume(Consumer<V> consumer) =>
            Cistern.Linq.Consume.ReadOnlySpan.Invoke(new ReadOnlySpan<T>(Underlying, _start, _length), Link, consumer);

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
            Optimizations.Count.TryGetCount(this, Link, asCountConsumer);
        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) =>
            _length;
    }
}
