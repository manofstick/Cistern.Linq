using System;
using System.Collections.Generic;
using Cistern.Linq.Consumables;

namespace Cistern.Linq.Consumer
{
    static class Lookup
    {
        private static Consumables.Lookup<TKey, TSource> GetLookupBuilder<TKey, TSource>(IEqualityComparer<TKey> comparer)
        {
            if (comparer == null || ReferenceEquals(comparer, EqualityComparer<TKey>.Default))
            {
                return new Consumables.LookupDefaultComparer<TKey, TSource>();
            }
            else
            {
                return new Consumables.LookupWithComparer<TKey, TSource>(comparer);
            }
        }

        internal static Consumables.Lookup<TKey, TSource> Consume<TKey, TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new Lookup<TSource, TKey>(builder, keySelector));
        }

        internal static Consumables.Lookup<TKey, TElement> Consume<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TElement> builder = GetLookupBuilder<TKey, TElement>(comparer);
            return Utils.Consume(source, new LookupSplit<TSource, TKey, TElement>(builder, keySelector, elementSelector));
        }

        internal static Consumables.Lookup<TKey, TSource> ConsumeForJoin<TKey, TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new LookupForJoin<TSource, TKey>(builder, keySelector, comparer));
        }
    }

    class LookupImpl
    {
        private static void AddItems<T, TKey>(Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> lookup, ref (T, T, T, T) items)
        {
            var key1 = keySelector(items.Item1);
            var key2 = keySelector(items.Item2);
            var key3 = keySelector(items.Item3);
            var key4 = keySelector(items.Item4);

            var grouping1 = lookup.GetGrouping(key1, create: true);
            var grouping2 = lookup.GetGrouping(key2, create: true);
            var grouping3 = lookup.GetGrouping(key3, create: true);
            var grouping4 = lookup.GetGrouping(key4, create: true);

            grouping1.Add(items.Item1);
            grouping2.Add(items.Item2);
            grouping3.Add(items.Item3);
            grouping4.Add(items.Item4);
        }

        private static void AddItem<T, TKey>(Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> lookup, T item)
        {
            var key = keySelector(item);
            var grouping = lookup.GetGrouping(key, create: true);
            grouping.Add(item);
        }

        private static void AddRemainingItems<T, TKey>(Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> lookup, int count, ref (T, T, T, T) items)
        {
            if (count > 0)
            {
                AddItem(keySelector, lookup, items.Item1);
                if (count > 1)
                {
                    AddItem(keySelector, lookup, items.Item2);
                    if (count > 2)
                    {
                        AddItem(keySelector, lookup, items.Item3);
                    }
                }
            }
        }

        private static void AddRemainingItems<S, T, TKey>(Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> lookup, int count, Func<S, T> selector, ref (S, S, S, S) items)
        {
            if (count > 0)
            {
                AddItem(keySelector, lookup, selector(items.Item1));
                if (count > 1)
                {
                    AddItem(keySelector, lookup, selector(items.Item2));
                    if (count > 2)
                    {
                        AddItem(keySelector, lookup, selector(items.Item3));
                    }
                }
            }
        }

        public static ChainStatus Execute<T, TKey>(ReadOnlySpan<T> source, Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> Result)
        {
            (T, T, T, T) items = default;

            var i = 0;
            for (; i < source.Length-4+1; i+=4)
            {
                items = (source[i+0], source[i+1], source[i+2], source[i+3]);
                AddItems(keySelector, Result, ref items);
            }

            for (; i < source.Length; ++i)
            {
                var item = source[i];
                AddItem(keySelector, Result, item);
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus Execute<T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            using var e = source.GetEnumerator();
            (T, T, T, T) items = default;

            int count = 0;
            var moveNext = e.MoveNext();
            while (moveNext)
            {
                items.Item1 = e.Current;
                count = 1;
                moveNext = e.MoveNext();
                if (moveNext)
                {
                    items.Item2 = e.Current;
                    count = 2;
                    moveNext = e.MoveNext();
                    if (moveNext)
                    {
                        items.Item3 = e.Current;
                        count = 3;
                        moveNext = e.MoveNext();
                        if (moveNext)
                        {
                            items.Item4 = e.Current;
                            count = 0;
                            AddItems(keySelector, Result, ref items);

                            moveNext = e.MoveNext();
                        }
                    }
                }
            }

            AddRemainingItems(keySelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus Select<S, T, TKey>(ReadOnlySpan<S> source, Func<T, TKey> keySelector, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
        {
            (T, T, T, T) items = default;

            var i = 0;
            for (; i < source.Length-4+1; i+=4)
            {
                items = (selector(source[i+0]), selector(source[i+1]), selector(source[i+2]), selector(source[i+3]));
                AddItems(keySelector, Result, ref items);
            }

            for (; i < source.Length; ++i)
            {
                var item = source[i];
                AddItem(keySelector, Result, selector(item));
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus Select<S, T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            using var e = source.GetEnumerator();
            (T, T, T, T) items = default;

            int count = 0;
            var moveNext = e.MoveNext();
            while (moveNext)
            {
                items.Item1 = selector(e.Current);
                moveNext = e.MoveNext();
                count = 1;
                if (moveNext)
                {
                    items.Item2 = selector(e.Current);
                    moveNext = e.MoveNext();
                    count = 2;
                    if (moveNext)
                    {
                        items.Item3 = selector(e.Current);
                        moveNext = e.MoveNext();
                        count = 3;
                        if (moveNext)
                        {
                            items.Item4 = selector(e.Current);
                            moveNext = e.MoveNext();
                            count = 0;

                            AddItems(keySelector, Result, ref items);
                        }
                    }
                }
            }

            AddRemainingItems(keySelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T, TKey>(ReadOnlySpan<T> source, Func<T, TKey> keySelector, Func<T, bool> predicate, Consumables.Lookup<TKey, T> Result)
        {
            (T, T, T, T) items = default;

            int count = 0;
            int i = 0;
            while (i < source.Length)
            {
                items.Item1 = source[i++];
                if (!predicate(items.Item1))
                    continue;
                count = 1;

                while (i < source.Length)
                {
                    items.Item2 = source[i++];
                    if (!predicate(items.Item2))
                        continue;
                    count = 2;

                    while (i < source.Length)
                    {
                        items.Item3 = source[i++];
                        if (!predicate(items.Item3))
                            continue;
                        count = 3;

                        while (i < source.Length)
                        {
                            items.Item4 = source[i++];
                            if (!predicate(items.Item4))
                                continue;
                            count = 0;

                            AddItems(keySelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<T, bool> predicate, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            using var e = source.GetEnumerator();
            (T, T, T, T) items = default;

            int count = 0;
            var moveNext = e.MoveNext();
            while (moveNext)
            {
                items.Item1 = e.Current;
                moveNext = e.MoveNext();
                if (!predicate(items.Item1))
                    continue;
                count = 1;
                while (moveNext)
                {
                    items.Item2 = e.Current;
                    moveNext = e.MoveNext();
                    if (!predicate(items.Item2))
                        continue;
                    count = 2;
                    while (moveNext)
                    {
                        items.Item3 = e.Current;
                        moveNext = e.MoveNext();
                        if (!predicate(items.Item3))
                            continue;
                        count = 3;
                        while (moveNext)
                        {
                            items.Item4 = e.Current;
                            moveNext = e.MoveNext();
                            if (!predicate(items.Item4))
                                continue;
                            count = 0;

                            AddItems(keySelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus SelectMany<T, TKey, TSource, TCollection>(TSource source, Func<T, TKey> keySelector, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector, Consumables.Lookup<TKey, T> Result)
        {
            var lookup = Result;
            foreach (var s in span)
            {
                var item = resultSelector(source, s);
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                grouping.Add(item);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T, TKey>(ReadOnlySpan<S> source, Func<T, TKey> keySelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
        {
            (S, S, S, S) itemsS = default;
            (T, T, T, T) items = default;

            int count = 0;
            int i = 0;
            while (i < source.Length)
            {
                itemsS.Item1 = source[i++];
                if (!predicate(itemsS.Item1))
                    continue;
                count = 1;

                while (i < source.Length)
                {
                    itemsS.Item2 = source[i++];
                    if (!predicate(itemsS.Item2))
                        continue;
                    count = 2;

                    while (i < source.Length)
                    {
                        itemsS.Item3 = source[i++];
                        if (!predicate(itemsS.Item3))
                            continue;
                        count = 3;

                        while (i < source.Length)
                        {
                            itemsS.Item4 = source[i++];
                            if (!predicate(itemsS.Item4))
                                continue;
                            count = 0;

                            items = (selector(itemsS.Item1), selector(itemsS.Item2), selector(itemsS.Item3), selector(itemsS.Item4));
                            AddItems(keySelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }
            
            AddRemainingItems(keySelector, Result, count, selector, ref itemsS);

            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            using var e = source.GetEnumerator();

            (S, S, S, S) itemsS = default;
            (T, T, T, T) items = default;

            int count = 0;
            var moveNext = e.MoveNext();
            while (moveNext)
            {
                itemsS.Item1 = e.Current;
                moveNext = e.MoveNext();
                if (!predicate(itemsS.Item1))
                    continue;
                count = 1;

                while (moveNext)
                {
                    itemsS.Item2 = e.Current;
                    moveNext = e.MoveNext();
                    if (!predicate(itemsS.Item2))
                        continue;
                    count = 2;

                    while (moveNext)
                    {
                        itemsS.Item3 = e.Current;
                        moveNext = e.MoveNext();
                        if (!predicate(itemsS.Item3))
                            continue;
                        count = 3;

                        while (moveNext)
                        {
                            itemsS.Item4 = e.Current;
                            moveNext = e.MoveNext();
                            if (!predicate(itemsS.Item4))
                                continue;
                            count = 0;

                            items = (selector(itemsS.Item1), selector(itemsS.Item2), selector(itemsS.Item3), selector(itemsS.Item4));
                            AddItems(keySelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, Result, count, selector, ref itemsS);

            return ChainStatus.Flow;
        }
    }

    sealed class Lookup<TSource, TKey>
        : Consumer<TSource, Consumables.Lookup<TKey, TSource>>
        , Optimizations.IHeadStart<TSource>
        , Optimizations.ITailEnd<TSource>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public Lookup(Consumables.Lookup<TKey, TSource> builder, Func<TSource, TKey> keySelector) : base(builder) =>
            (_keySelector) = (keySelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            Result.GetGrouping(_keySelector(item), create: true).Add(item);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<TSource>.Execute(ReadOnlySpan<TSource> source) =>
            LookupImpl.Execute(source, _keySelector, Result);

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source) =>
            LookupImpl.Execute<TSource, TKey, Enumerable, Enumerator>(source, _keySelector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Select<S>(ReadOnlySpan<S> source, Func<S, TSource> selector) =>
            LookupImpl.Select(source, _keySelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, TSource> selector) =>
            LookupImpl.Select<S, TSource, TKey, Enumerable, Enumerator>(source, _keySelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate) =>
            LookupImpl.Where(source, _keySelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where<Enumerable, Enumerator>(Enumerable source, Func<TSource, bool> predicate) =>
            LookupImpl.Where<TSource, TKey, Enumerable, Enumerator>(source, _keySelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.SelectMany<TSource1, TCollection>(TSource1 source, ReadOnlySpan<TCollection> span, Func<TSource1, TCollection, TSource> resultSelector) =>
            LookupImpl.SelectMany(source, _keySelector, span, resultSelector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.WhereSelect(source, _keySelector, predicate, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.WhereSelect<S, TSource, TKey, Enumerable, Enumerator>(source, _keySelector, predicate, selector, Result);
    }

    sealed class LookupSplit<TSource, TKey, TElement> : Consumer<TSource, Consumables.Lookup<TKey, TElement>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;

        public LookupSplit(Consumables.Lookup<TKey, TElement> builder, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) : base(builder) =>
            (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            Result.GetGrouping(_keySelector(item), create: true).Add(_elementSelector(item));
            return ChainStatus.Flow;
        }
    }

    sealed class LookupForJoin<TSource, TKey> : Consumer<TSource, Consumables.Lookup<TKey, TSource>>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public LookupForJoin(Consumables.Lookup<TKey, TSource> builder, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(builder) =>
            (_keySelector) = (keySelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            TKey key = _keySelector(item);
            if (key != null)
            {
                Result.GetGrouping(key, create: true).Add(item);
            }
            return ChainStatus.Flow;
        }
    }
}
