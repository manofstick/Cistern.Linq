using System;
using System.Collections.Generic;
using System.Threading;

namespace Cistern.Linq.Consumables
{
    static class AppendPrepend<T>
    {
        const int MinimumCellSize = 4;

        class SharedElements
        {
            public readonly T[] Elements;

            private int _interlocked_count;

            public SharedElements(T[] elements, int length) =>
                (Elements, _interlocked_count) = (elements, length);

            public bool TryOwn(int length) =>
                _interlocked_count == 0 || length == Interlocked.CompareExchange(ref _interlocked_count, length + 1, length);

            public static SharedElements Empty { get; } = new SharedElements(Array.Empty<T>(), 0);
        }

        public static IConsumable<T> AddElement(IEnumerable<T> unknown, T element, bool appendElsePrepend)
        {
            if (unknown is AppendPrependConsumable<T> append)
            {
                if (append.TryGetCell(out var cell, out var idx, appendElsePrepend))
                {
                    if (cell.TryOwn(idx))
                    {
                        var elements = cell.Elements;
                        if (idx < elements.Length)
                        {
                            if (appendElsePrepend)
                            {
                                elements[idx] = element;
                                return new AppendPrependConsumable<T>(append.Prepended, append.PrependedLength, append.Original, cell, idx + 1, Links.Identity<T>.Instance);
                            }
                            else
                            {
                                elements[elements.Length - 1 - idx] = element;
                                return new AppendPrependConsumable<T>(cell, idx + 1, append.Original, append.Appended, append.AppendedLength, Links.Identity<T>.Instance);
                            }
                        }

                        return CreateNewCellFromFullArray(append, cell.Elements, element, idx, appendElsePrepend);
                    }
                }
            }
            return CreateNew(unknown, element, appendElsePrepend);
        }

        private static IConsumable<T> CreateNew(IEnumerable<T> e, T element, bool appendElsePrepend)
        {
            var data = new T[MinimumCellSize];
            if (appendElsePrepend)
            {
                data[0] = element;
                var appended = new SharedElements(data, 1);
                return new AppendPrependConsumable<T>(SharedElements.Empty, 0, e, appended, 1, Links.Identity<T>.Instance);
            }
            else
            {
                data[data.Length-1] = element;
                var prepended = new SharedElements(data, 1);
                return new AppendPrependConsumable<T>(prepended, 1, e, SharedElements.Empty, 0, Links.Identity<T>.Instance);
            }
        }

        private static IConsumable<T> CreateNewCellFromFullArray(AppendPrependConsumable<T> root, T[] elements, T element, int length, bool appendElsePrepend)
        {
            var data = new T[Math.Max(MinimumCellSize, elements.Length * 2)];
            if (appendElsePrepend)
            {
                Array.Copy(elements, data, length);
                data[length] = element;
                var cell = new SharedElements(data, length + 1);

                return new AppendPrependConsumable<T>(root.Prepended, root.PrependedLength, root.Original, cell, length + 1, Links.Identity<T>.Instance);
            }
            else
            {
                Array.Copy(elements, 0, data, elements.Length, length);
                data[data.Length - length - 1] = element;
                var cell = new SharedElements(data, length + 1);

                return new AppendPrependConsumable<T>(cell, length + 1, root.Original, root.Appended, root.AppendedLength, Links.Identity<T>.Instance);
            }
        }

        sealed partial class AppendPrependConsumable<V>
            : Consumable<T, V>
            , Optimizations.IConsumableFastCount
        { 
            public AppendPrependConsumable(SharedElements prepended, int lengthPrepended, IEnumerable<T> e, SharedElements appended, int lengthAppended, ILink<T, V> link) : base(link)
            {
                Prepended = prepended;
                PrependedLength = lengthPrepended;
                Original = e;
                Appended = appended;
                AppendedLength = lengthAppended;
            }

            public SharedElements Prepended { get; }
            public int PrependedLength { get; }
            internal IEnumerable<T> Original { get; }
            public SharedElements Appended { get; }
            public int AppendedLength { get; }

            internal bool TryGetCell(out SharedElements cell, out int length, bool appendElsePrepend)
            {
                if (IsIdentity)
                {
                    if (appendElsePrepend)
                    {
                        cell = Appended;
                        length = AppendedLength;
                    }
                    else
                    {
                        cell = Prepended;
                        length = PrependedLength;
                    }
                    return true;
                }
                cell = default;
                length = default;
                return false;
            }

            IConsumable<T> GetAppendedPart() => new Array<T, T>(Appended.Elements, 0, AppendedLength, Links.Identity<T>.Instance);
            IConsumable<T> GetPrependedPart() => new Array<T, T>(Prepended.Elements, Prepended.Elements.Length - PrependedLength, PrependedLength, Links.Identity<T>.Instance);

            private IConsumable<IEnumerable<T>> GetEnumerableAsConsumable()
            {
                IEnumerable<T>[] joined;
                if (PrependedLength == 0)
                {
                    var appended = GetAppendedPart();
                    joined = new[] { Original, appended };
                }
                else if (AppendedLength == 0)
                {
                    var prepended = GetPrependedPart();
                    joined = new[] { prepended, Original };
                }
                else
                {
                    var prepended = GetPrependedPart();
                    var appended = GetAppendedPart();
                    joined = new[] { prepended, Original, appended };
                }
                return new Array<IEnumerable<T>, IEnumerable<T>>(joined, 0, joined.Length, Links.Identity<IEnumerable<T>>.Instance);
            }

            public override IConsumable<V> Create(ILink<T, V> link) => new AppendPrependConsumable<V>(Prepended, PrependedLength, Original, Appended, AppendedLength, link);
            public override IConsumable<W> Create<W>(ILink<T, W> link) => new AppendPrependConsumable<W>(Prepended, PrependedLength, Original, Appended, AppendedLength, link);

            public override IEnumerator<V> GetEnumerator() =>
                Cistern.Linq.GetEnumerator.SelectMany.Get(GetEnumerableAsConsumable(), Link);

            public override void Consume(Consumer<V> consumer) =>
                Cistern.Linq.Consume.SelectMany.Invoke(GetEnumerableAsConsumable(), Link, consumer);

            int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer)
            {
                checked
                {
                    var tryCount = Original switch
                    {
                        ICollection<T>                      x => x.Count,
                        Optimizations.IConsumableFastCount  x => x.TryFastCount(asCountConsumer),
                        System.Collections.ICollection      x => x.Count,
                        _                                     => null,
                    };
                    if (!tryCount.HasValue)
                        return null;

                    return PrependedLength + AppendedLength + tryCount.Value;
                }
            }
            int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) =>
                Optimizations.Count.TryGetCount(this, Link, asCountConsumer);
        }
    }
}
