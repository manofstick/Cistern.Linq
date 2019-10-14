using System;
using System.Collections.Generic;
using System.Threading;

namespace Cistern.Linq.Consumables
{
    static class Concat<T>
    {
        class Cell
        {
            public readonly IEnumerable<T>[] Enumerables;

            private int _interlocked_count;

            public Cell(IEnumerable<T>[] enumerables, int length) =>
                (Enumerables, _interlocked_count) = (enumerables, length);

            public bool TryOwn(int length) =>
                length == Interlocked.CompareExchange(ref _interlocked_count, length + 1, length);
        }

        public static IConsumable<T> Create(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first is ConcatConsumable<T> concat)
            {
                if (concat.TryGetCell(out var cell, out var idx))
                {
                    if (cell.TryOwn(idx))
                    {
                        var enumerables = cell.Enumerables;

                        if (idx < enumerables.Length)
                        {
                            enumerables[idx] = second;
                            return new ConcatConsumable<T>(cell, idx + 1, null);
                        }

                        return CreateNewCell(enumerables, second, idx);
                    }
                }
            }
            return CreateNew(first, second);
        }

        private static IConsumable<T> CreateNew(IEnumerable<T> first, IEnumerable<T> second)
        {
            var e = new Cell(new IEnumerable<T>[4] { first, second, null, null }, 2);
            return new ConcatConsumable<T>(e, 2, null);
        }

        private static IConsumable<T> CreateNewCell(IEnumerable<T>[] enumerables, IEnumerable<T> second, int length)
        {
            var data = new IEnumerable<T>[enumerables.Length * 2];
            Array.Copy(enumerables, data, length);
            data[length] = second;

            var cell = new Cell(data, length + 1);
            return new ConcatConsumable<T>(cell, length + 1, null);
        }

        sealed partial class ConcatConsumable<V>
            : Consumable<T, V>
            , Optimizations.IConsumableFastCount
        {
            Cell _enumerables;
            int _length;

            public ConcatConsumable(Cell enumerables, int length, ILink<T, V> link) : base(link)
            {
                _enumerables = enumerables;
                _length = length;
            }

            internal bool TryGetCell(out Cell cell, out int length)
            {
                if (IsIdentity)
                {
                    cell = _enumerables;
                    length = _length;
                    return true;
                }
                cell = default;
                length = default;
                return false;
            }

            private IConsumable<IEnumerable<T>> GetEnumerableAsConsumable() =>
                new Array<IEnumerable<T>, IEnumerable<T>>(_enumerables.Enumerables, 0, _length, null);

            public override IConsumable<V> Create(ILink<T, V> link) => new ConcatConsumable<V>(_enumerables, _length, link);
            public override IConsumable<W> Create<W>(ILink<T, W> link) => new ConcatConsumable<W>(_enumerables, _length, link);

            public override IEnumerator<V> GetEnumerator() =>
                Cistern.Linq.GetEnumerator.SelectMany.Get(GetEnumerableAsConsumable(), Link);

            public override void Consume(Consumer<V> consumer) =>
                Cistern.Linq.Consume.SelectMany.Invoke(GetEnumerableAsConsumable(), Link, consumer);

            int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer)
            {
                checked
                {
                    var count = 0;
                    for (var i = 0; i < _length; ++i)
                    {
                        var tryCount = _enumerables.Enumerables[i] switch
                        {
                            ICollection<T>                      x => x.Count,
                            Optimizations.IConsumableFastCount  x => x.TryFastCount(asCountConsumer),
                            System.Collections.ICollection      x => x.Count,
                            _                                     => null,
                        };
                        if (!tryCount.HasValue)
                            return null;

                        count += tryCount.Value;
                    }
                    return count;
                }
            }
            int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
                Optimizations.Count.TryGetCount(this, Link, asCountConsumer);
        }

    }
}
