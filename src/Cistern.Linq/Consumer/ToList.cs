using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{
    static class ToListImpl
    {
        private static void EnsureCapacity<T>(List<T> list, int length)
        {
            list.Capacity = list.Count + length;
        }

        private static void EnsureCapacity<T>(List<T> list, int? maybeLength)
        {
            if (maybeLength.HasValue)
            {
                list.Capacity = list.Count + maybeLength.Value;
            }
        }

        public static ChainStatus Execute<T>(ReadOnlySpan<T> source, List<T> result)
        {
            EnsureCapacity(result, source.Length);
            foreach (var input in source)
            {
                result.Add(input);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Execute<T, Enumerable, Enumerator>(Enumerable source, List<T> result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>
        {
            EnsureCapacity(result, source.TryLength);
            foreach (var input in source)
            {
                result.Add(input);
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Select<S, T>(ReadOnlySpan<S> source, List<T> result, Func<S, T> selector)
        {
            EnsureCapacity(result, source.Length);
            var s = selector;
            foreach (var input in source)
            {
                result.Add(s(input));
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Select<S, T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>
        {
            var maybeLength = source.TryLength;
            if (maybeLength.HasValue)
                EnsureCapacity(result, maybeLength.Value);

            var s = selector;
            foreach (var input in source)
            {
                result.Add(s(input));
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus SelectMany<T, TSource, TCollection>(TSource source, List<T> result, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            EnsureCapacity(result, span.Length);
            var rs = resultSelector;
            foreach (var input in span)
            {
                result.Add(rs(source, input));
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T>(ReadOnlySpan<T> source, List<T> result, Func<T, bool> predicate)
        {
            var p = predicate;
            foreach (var input in source)
            {
                if (p(input))
                {
                    result.Add(input);
                }
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus Where<T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<T, bool> predicate)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>
        {
            var p = predicate;
            foreach (var input in source)
            {
                if (p(input))
                {
                    result.Add(input);
                }
            }

            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T>(ReadOnlySpan<S> source, List<T> result, Func<S, bool> predicate, Func<S, T> selector)
        {
            var p = predicate;
            var s = selector;
            foreach (var input in source)
            {
                if (p(input))
                {
                    result.Add(s(input));
                }
            }
            return ChainStatus.Flow;
        }

        public static ChainStatus WhereSelect<S, T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<S, bool> predicate, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>
        {
            var p = predicate;
            var s = selector;
            foreach (var input in source)
            {
                if (p(input))
                {
                    result.Add(s(input));
                }
            }
            return ChainStatus.Flow;
        }
    }

    sealed class ToList<T>
        : Consumer<T, List<T>>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
    {
        public ToList() : base(new List<T>()) { }

        public ToList(int count) : base(new List<T>(count)) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source) =>
            ToListImpl.Execute(source, Result);

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source) =>
            ToListImpl.Execute<T, Enumerable, Enumerator>(source, Result);

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector) =>
            ToListImpl.Select(source, Result, selector);

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector) =>
            ToListImpl.Select<S, T, Enumerable, Enumerator>(source, Result, selector);

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector) =>
            ToListImpl.SelectMany(source, Result, span, resultSelector);

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate) =>
            ToListImpl.Where(source, Result, predicate);

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate) =>
            ToListImpl.Where<T, Enumerable, Enumerator>(source, Result, predicate);

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector) =>
            ToListImpl.WhereSelect(source, Result, predicate, selector);
        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector) =>
            ToListImpl.WhereSelect<S, T, Enumerable, Enumerator>(source, Result, predicate, selector);
    }
}
