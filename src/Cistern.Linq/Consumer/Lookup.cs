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
        public static ChainStatus Invoke<T, TKey>(ReadOnlySpan<T> source, Func<T, TKey> _keySelector, Consumables.Lookup<TKey, T> Result)
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var item in source)
            {
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                grouping.Add(item);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> _keySelector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var item in source)
            {
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                grouping.Add(item);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<S, T, TKey>(ReadOnlySpan<S> source, Func<T, TKey> _keySelector, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var s in source)
            {
                var item = selector(s);
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                grouping.Add(item);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<S, T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> _keySelector, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var s in source)
            {
                var item = selector(s);
                var key = keySelector(item);
                var grouping = lookup.GetGrouping(key, create: true);
                grouping.Add(item);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<T, TKey>(ReadOnlySpan<T> source, Func<T, TKey> _keySelector, Func<T, bool> predicate, Consumables.Lookup<TKey, T> Result)
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    var key = keySelector(item);
                    var grouping = lookup.GetGrouping(key, create: true);
                    grouping.Add(item);
                }
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> _keySelector, Func<T, bool> predicate, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    var key = keySelector(item);
                    var grouping = lookup.GetGrouping(key, create: true);
                    grouping.Add(item);
                }
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<T, TKey, TSource, TCollection>(TSource source, Func<T, TKey> _keySelector, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector, Consumables.Lookup<TKey, T> Result)
        {
            var keySelector = _keySelector;
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

        public static ChainStatus Invoke<S, T, TKey>(ReadOnlySpan<S> source, Func<T, TKey> _keySelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var s in source)
            {
                if (predicate(s))
                {
                    var item = selector(s);
                    var key = keySelector(item);
                    var grouping = lookup.GetGrouping(key, create: true);
                    grouping.Add(item);
                }
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Invoke<S, T, TKey, Enumerable, Enumerator>(Enumerable source, Func<T, TKey> _keySelector, Func<S, bool> predicate, Func<S, T> selector, Consumables.Lookup<TKey, T> Result)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            var keySelector = _keySelector;
            var lookup = Result;
            foreach (var s in source)
            {
                if (predicate(s))
                {
                    var item = selector(s);
                    var key = keySelector(item);
                    var grouping = lookup.GetGrouping(key, create: true);
                    grouping.Add(item);
                }
            }
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
            LookupImpl.Invoke(source, _keySelector, Result);

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source) =>
            LookupImpl.Invoke<TSource, TKey, Enumerable, Enumerator>(source, _keySelector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Select<S>(ReadOnlySpan<S> source, Func<S, TSource> selector) =>
            LookupImpl.Invoke(source, _keySelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, TSource> selector) =>
            LookupImpl.Invoke<S, TSource, TKey, Enumerable, Enumerator>(source, _keySelector, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate) =>
            LookupImpl.Invoke(source, _keySelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.Where<Enumerable, Enumerator>(Enumerable source, Func<TSource, bool> predicate) =>
            LookupImpl.Invoke<TSource, TKey, Enumerable, Enumerator>(source, _keySelector, predicate, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.SelectMany<TSource1, TCollection>(TSource1 source, ReadOnlySpan<TCollection> span, Func<TSource1, TCollection, TSource> resultSelector) =>
            LookupImpl.Invoke(source, _keySelector, span, resultSelector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.Invoke(source, _keySelector, predicate, selector, Result);

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, TSource> selector) =>
            LookupImpl.Invoke<S, TSource, TKey, Enumerable, Enumerator>(source, _keySelector, predicate, selector, Result);
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
