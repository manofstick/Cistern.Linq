using System;
using System.Collections.Generic;
using System.Threading;

namespace Cistern.Linq.ChainLinq.Consumables
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

        public static Consumable<T> Create(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first is ConcatConsumable<T> concat && concat.TryGetCell(out var cell, out var length))
            {
                if (cell.TryOwn(length))
                {
                    if (length < cell.Enumerables.Length)
                    {
                        cell.Enumerables[length] = second;
                        return new ConcatConsumable<T>(cell, length + 1, Links.Identity<T>.Instance);
                    }

                    var data = new IEnumerable<T>[cell.Enumerables.Length * 2];
                    Array.Copy(cell.Enumerables, data, length);
                    data[length] = second;

                    var enumerables = new Cell(data, length + 1);
                    return new ConcatConsumable<T>(enumerables, length + 1, Links.Identity<T>.Instance);
                }
            }

            var e = new Cell(new IEnumerable<T>[4] { first, second, null, null }, 2);
            return new ConcatConsumable<T>(e, 2, Links.Identity<T>.Instance);
        }

        sealed partial class ConcatConsumable<V>
            : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
            , Optimizations.ICountOnConsumable
        {
            Cell _enumerables;
            int _length;

            public ConcatConsumable(Cell enumerables, int length, Link<T, V> link) : base(link)
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

            private Consumable<IEnumerable<T>> GetEnumerableAsConsumable() =>
                new Array<IEnumerable<T>, IEnumerable<T>>(_enumerables.Enumerables, 0, _length, Links.Identity<IEnumerable<T>>.Instance);

            public override Consumable<V> Create(Link<T, V> link) => new ConcatConsumable<V>(_enumerables, _length, link);
            public override Consumable<W> Create<W>(Link<T, W> link) => new ConcatConsumable<W>(_enumerables, _length, link);

            public override IEnumerator<V> GetEnumerator() =>
                ChainLinq.GetEnumerator.SelectMany.Get(GetEnumerableAsConsumable(), Link);

            public override void Consume(Consumer<V> consumer) =>
                ChainLinq.Consume.SelectMany.Invoke(GetEnumerableAsConsumable(), Link, consumer);

            int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap)
            {
                checked
                {
                    if (Link is Optimizations.ICountOnConsumableLink countLink)
                    {
                        var count = 0;

                        for (var i = 0; i < _length; ++i)
                        {
                            var e = _enumerables.Enumerables[i];

                            var tryCount = TryCount(e, onlyIfCheap);
                            if (tryCount < 0)
                            {
                                if (onlyIfCheap)
                                    return -1;

                                return FullCount();
                            }

                            count += tryCount;

                        }
                        return countLink.GetCount(count);
                    }

                    if (onlyIfCheap)
                    {
                        return -1;
                    }

                    return FullCount();
                }
            }

            private int FullCount()
            {
                var counter = new Consumer.Count<V, int, int, Maths.OpsInt>();
                Consume(counter);
                return counter.Result;
            }

            private static int TryCount(IEnumerable<T> e, bool onlyIfCheap)
            {
                if (e is ICollection<T> ct)
                {
                    return ct.Count;
                }
                else if (e is Optimizations.ICountOnConsumable cc)
                {
                    return cc.GetCount(onlyIfCheap);
                }
                else if (e is System.Collections.ICollection c)
                {
                    return c.Count;
                }
                else
                {
                    return -1;
                }
            }
        }

    }
}
