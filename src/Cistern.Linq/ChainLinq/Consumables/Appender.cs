using System;
using System.Collections.Generic;
using System.Threading;

namespace Cistern.Linq.ChainLinq.Consumables
{
    static class Append<T>
    {
        class Appended
        {
            public readonly T[] Elements;

            private int _interlocked_count;

            public Appended(T[] elements, int length) =>
                (Elements, _interlocked_count) = (elements, length);

            public bool TryOwn(int length) =>
                length == Interlocked.CompareExchange(ref _interlocked_count, length + 1, length);
        }

        public static Consumable<T> Create(IEnumerable<T> unknown, T element)
        {
            if (unknown is AppendConsumable<T> append)
            {
                if (append.TryGetCell(out var cell, out var idx))
                {
                    if (cell.TryOwn(idx))
                    {
                        var elements = cell.Elements;
                        if (idx < elements.Length)
                        {
                            elements[idx] = element;
                            return new AppendConsumable<T>(append.Root, cell, idx + 1, Links.Identity<T>.Instance);
                        }

                        return CreateNewCellFromFullArray(append.Root, cell.Elements, element, idx);
                    }
                }
            }
            return CreateNew(unknown, element);
        }

        private static Consumable<T> CreateNew(IEnumerable<T> e, T element)
        {
            var data = new T[4];
            data[0] = element;
            var appended = new Appended(data, 1);
            return new AppendConsumable<T>(e, appended, 1, Links.Identity<T>.Instance);
        }

        private static Consumable<T> CreateNewCellFromFullArray(IEnumerable<T> root, T[] elements, T element, int length)
        {
            var data = new T[elements.Length * 2];
            Array.Copy(elements, data, length);
            data[length] = element;
            var cell = new Appended(data, length + 1);

            return new AppendConsumable<T>(root, cell, length + 1, Links.Identity<T>.Instance);
        }

        sealed partial class AppendConsumable<V>
            : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
            , Optimizations.IConsumableFastCount
        {
            private readonly IEnumerable<T> _enumerable;
            private readonly Appended _appended;
            private readonly int _length;

            public AppendConsumable(IEnumerable<T> e, Appended appended, int length, ILink<T, V> link) : base(link)
            {
                _enumerable = e;
                _appended = appended;
                _length = length;
            }

            internal IEnumerable<T> Root => _enumerable;

            internal bool TryGetCell(out Appended cell, out int length)
            {
                if (IsIdentity)
                {
                    cell = _appended;
                    length = _length;
                    return true;
                }
                cell = default;
                length = default;
                return false;
            }

            private Consumable<IEnumerable<T>> GetEnumerableAsConsumable()
            {
                IEnumerable<T> elements = new Array<T, T>(_appended.Elements, 0, _length, Links.Identity<T>.Instance);
                var joined = new[] { _enumerable, elements };
                return new Array<IEnumerable<T>, IEnumerable<T>>(joined, 0, joined.Length, Links.Identity<IEnumerable<T>>.Instance);
            }

            public override Consumable<V> Create(ILink<T, V> link) => new AppendConsumable<V>(_enumerable, _appended, _length, link);
            public override Consumable<W> Create<W>(ILink<T, W> link) => new AppendConsumable<W>(_enumerable, _appended, _length, link);

            public override IEnumerator<V> GetEnumerator() =>
                ChainLinq.GetEnumerator.SelectMany.Get(GetEnumerableAsConsumable(), Link);

            public override void Consume(Consumer<V> consumer) =>
                ChainLinq.Consume.SelectMany.Invoke(GetEnumerableAsConsumable(), Link, consumer);

            int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer)
            {
                checked
                {
                    var tryCount = _enumerable switch
                    {
                        ICollection<T>                      x => x.Count,
                        Optimizations.IConsumableFastCount  x => x.TryFastCount(asCountConsumer),
                        System.Collections.ICollection      x => x.Count,
                        _                                     => null,
                    };
                    if (!tryCount.HasValue)
                        return null;

                    return _length + tryCount.Value;
                }
            }
            int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
                Optimizations.Count.TryGetCount(this, Link, asCountConsumer);
        }
    }
}
