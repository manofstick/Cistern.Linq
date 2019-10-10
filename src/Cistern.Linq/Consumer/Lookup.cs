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

        internal static Consumables.Lookup<TKey, TElement> Consume<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TElement> builder = GetLookupBuilder<TKey, TElement>(comparer);
            return Utils.Consume(source, new Lookup<TSource, TKey, TElement>(builder, keySelector, elementSelector, false));
        }

        internal static Consumables.Lookup<TKey, TSource> ConsumeForJoin<TKey, TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new Lookup<TSource, TKey, TSource>(builder, keySelector, x=>x, true));
        }
    }

#if LookupImpl_IHeadStart

    static partial class LookupImpl
    {
        private static void AddItems<T, TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> lookup, ref (T, T, T, T) items)
        {
            var key1 = keySelector(items.Item1);
            var key2 = keySelector(items.Item2);
            var key3 = keySelector(items.Item3);
            var key4 = keySelector(items.Item4);

            var grouping1 = lookup.GetGrouping(key1, create: true);
            var grouping2 = lookup.GetGrouping(key2, create: true);
            var grouping3 = lookup.GetGrouping(key3, create: true);
            var grouping4 = lookup.GetGrouping(key4, create: true);

            var element1 = elementSelector(items.Item1);
            var element2 = elementSelector(items.Item2);
            var element3 = elementSelector(items.Item3);
            var element4 = elementSelector(items.Item4);

            grouping1.Add(element1);
            grouping2.Add(element2);
            grouping3.Add(element3);
            grouping4.Add(element4);
        }

        private static void AddItem<T, TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> lookup, T item)
        {
            var key = keySelector(item);
            var grouping = lookup.GetGrouping(key, create: true);
            var element = elementSelector(item);
            grouping.Add(element);
        }

        private static void AddRemainingItems<T, TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> lookup, int count, ref (T, T, T, T) items)
        {
            if (count >= 1)
            {
                AddItem(keySelector, elementSelector, lookup, items.Item1);
                if (count >= 2)
                {
                    AddItem(keySelector, elementSelector, lookup, items.Item2);
                    if (count >= 3)
                    {
                        AddItem(keySelector, elementSelector, lookup, items.Item3);
                    }
                }
            }
        }

        private static void AddRemainingItems<S, T, TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> lookup, int count, Func<S, T> selector, ref (S, S, S, S) items)
        {
            if (count >= 1)
            {
                AddItem(keySelector, elementSelector, lookup, selector(items.Item1));
                if (count >= 2)
                {
                    AddItem(keySelector, elementSelector, lookup, selector(items.Item2));
                    if (count >= 3)
                    {
                        AddItem(keySelector, elementSelector, lookup, selector(items.Item3));
                    }
                }
            }
        }

        public static ChainStatus Execute<T, TKey, TElement>(ReadOnlySpan<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> Result)
        {
            (T, T, T, T) items = default;

            var i = 0;
            for (; i < source.Length - 4 + 1; i += 4)
            {
                items = (source[i + 0], source[i + 1], source[i + 2], source[i + 3]);
                AddItems(keySelector, elementSelector, Result, ref items);
            }

            for (; i < source.Length; ++i)
            {
                var item = source[i];
                AddItem(keySelector, elementSelector, Result, item);
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus Execute<T, TKey, TElement, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Consumables.Lookup<TKey, TElement> Result)
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
                            AddItems(keySelector, elementSelector, Result, ref items);

                            moveNext = e.MoveNext();
                        }
                    }
                }
            }

            AddRemainingItems(keySelector, elementSelector, Result, count, ref items);

            return ChainStatus.Flow;
        }
    }

    sealed partial class Lookup<TSource, TKey, TElement>
        : Optimizations.IHeadStart<TSource>
    {
        ChainStatus Optimizations.IHeadStart<TSource>.Execute(ReadOnlySpan<TSource> source) =>
            LookupImpl.Execute(source, _keySelector, _elementSelector, Result);

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source) =>
            LookupImpl.Execute<TSource, TKey, TElement, Enumerable, Enumerator>(source, _keySelector, _elementSelector, Result);
    }
#endif

#if LookupImpl_ITailEnd

    static partial class LookupImpl
    { 
        public static ChainStatus Select<S, T, TKey, TElement>(ReadOnlySpan<S> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<S, T> selector, Consumables.Lookup<TKey, TElement> Result)
        {
            (T, T, T, T) items = default;

            var i = 0;
            for (; i < source.Length-4+1; i+=4)
            {
                items = (selector(source[i+0]), selector(source[i+1]), selector(source[i+2]), selector(source[i+3]));
                AddItems(keySelector, elementSelector, Result, ref items);
            }

            for (; i < source.Length; ++i)
            {
                var item = source[i];
                AddItem(keySelector, elementSelector, Result, selector(item));
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus Select<S, T, TKey, TElement, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<S, T> selector, Consumables.Lookup<TKey, TElement> Result)
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

                            AddItems(keySelector, elementSelector, Result, ref items);
                        }
                    }
                }
            }

            AddRemainingItems(keySelector, elementSelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T, TKey, TElement>(ReadOnlySpan<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<T, bool> predicate, Consumables.Lookup<TKey, TElement> Result)
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

                            AddItems(keySelector, elementSelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, elementSelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T, TKey, TElement, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<T, bool> predicate, Consumables.Lookup<TKey, TElement> Result)
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

                            AddItems(keySelector, elementSelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, elementSelector, Result, count, ref items);

            return ChainStatus.Flow;
        }

        public static ChainStatus SelectMany<T, TKey, TElement, TSource, TCollection>(TSource source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector, Consumables.Lookup<TKey, TElement> Result)
        {
            var lookup = Result;
            foreach (var s in span)
            {
                var item = resultSelector(source, s);
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                var element = elementSelector(item);
                grouping.Add(element);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T, TKey, TElement>(ReadOnlySpan<S> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, TElement> Result)
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
                            AddItems(keySelector, elementSelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }
            
            AddRemainingItems(keySelector, elementSelector, Result, count, selector, ref itemsS);

            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T, TKey, TElement, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, TElement> Result)
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
                            AddItems(keySelector, elementSelector, Result, ref items);

                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(keySelector, elementSelector, Result, count, selector, ref itemsS);

            return ChainStatus.Flow;
        }
    }

    sealed partial class Lookup<TSource, TKey, TElement>
        : Optimizations.ITailEnd<TSource>
    {
        ChainStatus Optimizations.ITailEnd<TSource>.Select<S>(ReadOnlySpan<S> source, Func<S, TSource> selector) =>
            LookupImpl.Select(source, _keySelector, _elementSelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, TSource> selector) =>
            LookupImpl.Select<S, TSource, TKey, TElement, Enumerable, Enumerator>(source, _keySelector, _elementSelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate) =>
            LookupImpl.Where(source, _keySelector, _elementSelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where<Enumerable, Enumerator>(Enumerable source, Func<TSource, bool> predicate) =>
            LookupImpl.Where<TSource, TKey, TElement, Enumerable, Enumerator>(source, _keySelector, _elementSelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.SelectMany<TSource1, TCollection>(TSource1 source, ReadOnlySpan<TCollection> span, Func<TSource1, TCollection, TSource> resultSelector) =>
            LookupImpl.SelectMany(source, _keySelector, _elementSelector, span, resultSelector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.WhereSelect(source, _keySelector, _elementSelector, predicate, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.WhereSelect<S, TSource, TKey, TElement, Enumerable, Enumerator>(source, _keySelector, _elementSelector, predicate, selector, Result);
    }
#endif

    static partial class LookupImpl
    {
        private static void AddItem<TKey, TElement>(Consumables.Lookup<TKey, TElement> lookup, TKey key, TElement element) =>
            lookup.GetGrouping(key, create: true).Add(element);

        internal static void AddItems<TKey, TElement>(Consumables.Lookup<TKey, TElement> lookup, ref (TKey, TKey, TKey, TKey) keys, ref (TElement, TElement, TElement, TElement) elements)
        {
            var grouping1 = lookup.GetGrouping(keys.Item1, create: true);
            var grouping2 = lookup.GetGrouping(keys.Item2, create: true);
            var grouping3 = lookup.GetGrouping(keys.Item3, create: true);
            var grouping4 = lookup.GetGrouping(keys.Item4, create: true);

            grouping1.Add(elements.Item1);
            grouping2.Add(elements.Item2);
            grouping3.Add(elements.Item3);
            grouping4.Add(elements.Item4);
        }

        internal static void AddRemainingItems<TKey, TElement>(Consumables.Lookup<TKey, TElement> result, int count, ref (TKey, TKey, TKey, TKey) keys, ref (TElement, TElement, TElement, TElement) elements)
        {
            if (count >= 1)
            {
                AddItem(result, keys.Item1, elements.Item1);
                if (count >= 2)
                {
                    AddItem(result, keys.Item2, elements.Item2);
                    if (count >= 3)
                    {
                        AddItem(result, keys.Item3, elements.Item3);
                    }
                }
            }

        }
    }

    sealed partial class Lookup<TSource, TKey, TElement>
        : Consumer<TSource, Consumables.Lookup<TKey, TElement>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly bool _ignoreNullKeys;

        private int _count;
        private (TKey, TKey, TKey, TKey) _keys;
        private (TElement, TElement, TElement, TElement) _elements;

        public Lookup(Consumables.Lookup<TKey, TElement> builder, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, bool ignoreNullKeys) : base(builder) =>
            (_keySelector, _elementSelector, _ignoreNullKeys) = (keySelector, elementSelector, ignoreNullKeys);

        public override ChainStatus ProcessNext(TSource item)
        {
            TKey key = _keySelector(item);
            if (!(_ignoreNullKeys && key == null))
            {
                switch (_count)
                {
                    case 0:
                        _keys.Item1 = key;
                        _elements.Item1 = _elementSelector(item);
                        _count = 1;
                        break;

                    case 1:
                        _keys.Item2 = key;
                        _elements.Item2 = _elementSelector(item);
                        _count = 2;
                        break;

                    case 2:
                        _keys.Item3 = key;
                        _elements.Item3 = _elementSelector(item);
                        _count = 3;
                        break;

                    default:
                        _keys.Item4 = key;
                        _elements.Item4 = _elementSelector(item);

                        LookupImpl.AddItems(Result, ref _keys, ref _elements);

                        _count = 0;
                        break;
                }
            }
            return ChainStatus.Flow;
        }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            LookupImpl.AddRemainingItems(Result, _count, ref _keys, ref _elements);
            return ChainStatus.Filter;
        }
    }
}
