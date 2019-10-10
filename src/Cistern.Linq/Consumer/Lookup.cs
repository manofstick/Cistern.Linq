using System;
using System.Collections.Generic;

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

        internal static Consumables.Lookup<TKey, TSource> Consume<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new Lookup<TSource, TKey>(builder, keySelector));
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

    static partial class LookupImpl
    {
        public static void AddItem<TKey, TElement>(Consumables.Lookup<TKey, TElement> lookup, TKey key, TElement element) =>
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
        public static void Execute<TSource, TKey>(ReadOnlySpan<TSource> source, Func<TSource, TKey> getKey, Consumables.Lookup<TKey, TSource> result)
        {
            var i = 0;
            for (; i < source.Length - 4 + 1; i += 4)
            {
                var elements = (source[i + 0], source[i + 1], source[i + 2], source[i + 3]);
                var keys = (getKey(elements.Item1), getKey(elements.Item2), getKey(elements.Item3), getKey(elements.Item4));

                AddItems(result, ref keys, ref elements);
            }

            for (; i < source.Length; ++i)
            {
                var item = source[i];

                AddItem(result, getKey(item), item);
            }
        }

        public static void Execute<TSource, TKey, TElement>(ReadOnlySpan<TSource> source, Func<TSource, TKey> getKey, Func<TSource, TElement> getElement, bool _ignoreNullKeys, Consumables.Lookup<TKey, TElement> result)
        {
            var idx = 0;
            var count = 0;

            TSource item;
            (TKey, TKey, TKey, TKey) keys = default;
            (TElement, TElement, TElement, TElement) elements = default;
            while (idx < source.Length)
            {
                item = source[idx++];
                keys.Item1 = getKey(item);
                if (_ignoreNullKeys && keys.Item1 == null)
                    continue;

                elements.Item1 = getElement(item);
                count = 1;
                while (idx < source.Length)
                {
                    item = source[idx++];
                    keys.Item2 = getKey(item);
                    if (_ignoreNullKeys && keys.Item2 == null)
                        continue;

                    elements.Item2 = getElement(item);
                    count = 2;
                    while (idx < source.Length)
                    {
                        item = source[idx++];
                        keys.Item3 = getKey(item);
                        if (_ignoreNullKeys && keys.Item3 == null)
                            continue;

                        elements.Item3 = getElement(item);
                        count = 3;
                        while (idx < source.Length)
                        {
                            item = source[idx++];
                            keys.Item4 = getKey(item);
                            if (_ignoreNullKeys && keys.Item4 == null)
                                continue;

                            elements.Item4 = getElement(item);

                            AddItems(result, ref keys, ref elements);

                            count = 0;
                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(result, count, ref keys, ref elements);
        }

        public static void Execute<TSource, TKey, TElement, Enumerable, Enumerator>(Enumerable source, Func<TSource, TKey> getKey, Func<TSource, TElement> getElement, bool ignoreNullKeys, Consumables.Lookup<TKey, TElement> result)
            where Enumerable : Optimizations.ITypedEnumerable<TSource, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<TSource>
        {
            int count = 0;

            TSource item;
            (TKey, TKey, TKey, TKey) keys = default;
            (TElement, TElement, TElement, TElement) elements = default;
            using var e = source.GetEnumerator();
            var moveNext = e.MoveNext();
            while (moveNext)
            {
                item = e.Current;
                moveNext = e.MoveNext();
                keys.Item1 = getKey(item);
                if (ignoreNullKeys && keys.Item1 == null)
                    continue;

                elements.Item1 = getElement(item);
                count = 1;
                while (moveNext)
                {
                    item = e.Current;
                    moveNext = e.MoveNext();
                    keys.Item2 = getKey(item);
                    if (ignoreNullKeys && keys.Item2 == null)
                        continue;

                    elements.Item2 = getElement(item);
                    count = 2;
                    while (moveNext)
                    {
                        item = e.Current;
                        moveNext = e.MoveNext();
                        keys.Item3 = getKey(item);
                        if (ignoreNullKeys && keys.Item3 == null)
                            continue;

                        elements.Item3 = getElement(item);
                        count = 3;
                        while (moveNext)
                        {
                            item = e.Current;
                            moveNext = e.MoveNext();
                            keys.Item4 = getKey(item);
                            if (ignoreNullKeys && keys.Item4 == null)
                                continue;

                            elements.Item4 = getElement(item);

                            AddItems(result, ref keys, ref elements);

                            count = 0;
                            break;
                        }
                        break;
                    }
                    break;
                }
            }

            AddRemainingItems(result, count, ref keys, ref elements);
        }
    }

    class Lookup<TSource, TKey, TElement>
        : Consumer<TSource, Consumables.Lookup<TKey, TElement>>
        , Optimizations.IHeadStart<TSource>
    {
        protected readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly bool _ignoreNullKeys;

        private int _count;
        private (TKey, TKey, TKey, TKey) _keys;
        private (TElement, TElement, TElement, TElement) _elements;

        public Lookup(Consumables.Lookup<TKey, TElement> builder, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, bool ignoreNullKeys) : base(builder) =>
            (_keySelector, _elementSelector, _ignoreNullKeys) = (keySelector, elementSelector, ignoreNullKeys);

        protected void Flush()
        {
            LookupImpl.AddRemainingItems(Result, _count, ref _keys, ref _elements);

            _count = 0;
            _keys = default;
            _elements = default;
        }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Flush();
            return ChainStatus.Filter;
        }

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

        public virtual ChainStatus Execute(ReadOnlySpan<TSource> source)
        {
            Flush();
            LookupImpl.Execute(source, _keySelector, _elementSelector, _ignoreNullKeys, Result);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Flush();
            LookupImpl.Execute<TSource, TKey, TElement, Enumerable, Enumerator>(source, _keySelector, _elementSelector, _ignoreNullKeys, Result);
            return ChainStatus.Filter;
        }
    }

    class Lookup<TSource, TKey>
        : Lookup<TSource, TKey, TSource>
    {
        public Lookup(Consumables.Lookup<TKey, TSource> builder, Func<TSource, TKey> keySelector) 
            : base(builder, keySelector, x => x, false) { }

        public override ChainStatus Execute(ReadOnlySpan<TSource> source)
        {
            Flush();
            LookupImpl.Execute(source, _keySelector, Result);
            return ChainStatus.Filter;
        }
    }
}
