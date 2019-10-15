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

        public static void Execute<T>(ReadOnlySpan<T> source, List<T> result)
        {
            EnsureCapacity(result, source.Length);
            foreach (var input in source)
            {
                result.Add(input);
            }
        }

        public static void Execute<T, Enumerable, Enumerator>(Enumerable source, List<T> result)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>
        {
            EnsureCapacity(result, source.TryLength);
            foreach (var input in source)
            {
                result.Add(input);
            }
        }

        public static void Select<S, T>(ReadOnlySpan<S> source, List<T> result, Func<S, T> selector)
        {
            EnsureCapacity(result, source.Length);
            foreach (var input in source)
            {
                result.Add(selector(input));
            }
        }

        public static void Select<S, T>(List<S> source, List<T> result, Func<S, T> selector)
        {
            EnsureCapacity(result, source.Count);
            for(var i=0; i < source.Count; ++i)
            {
                result.Add(selector(source[i]));
            }
        }

        public static void Select<S, T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>
        {
            var maybeLength = source.TryLength;
            if (maybeLength.HasValue)
                EnsureCapacity(result, maybeLength.Value);

            foreach (var input in source)
            {
                result.Add(selector(input));
            }
        }

        public static void SelectMany<T, TSource, TCollection>(TSource source, List<T> result, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            EnsureCapacity(result, span.Length);
            foreach (var input in span)
            {
                result.Add(resultSelector(source, input));
            }
        }

        public static void Where<T>(ReadOnlySpan<T> source, List<T> result, Func<T, bool> predicate)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(input);
                }
            }
        }
        public static void Where<T>(List<T> source, List<T> result, Func<T, bool> predicate)
        {
            for(var i=0; i < source.Count; ++i)
            {
                var input = source[i];
                if (predicate(input))
                {
                    result.Add(input);
                }
            }
        }

        public static void Where<T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<T, bool> predicate)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(input);
                }
            }
        }

        public static void WhereSelect<S, T>(ReadOnlySpan<S> source, List<T> result, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(selector(input));
                }
            }
        }
        public static void WhereSelect<S, T>(List<S> source, List<T> result, Func<S, bool> predicate, Func<S, T> selector)
        {
            for(var i=0; i < source.Count; ++i)
            {
                var input = source[i];
                if (predicate(input))
                {
                    result.Add(selector(input));
                }
            }
        }

        public static void WhereSelect<S, T, Enumerable, Enumerator>(Enumerable source, List<T> result, Func<S, bool> predicate, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(selector(input));
                }
            }
        }

        public static void SelectWhere<S, T>(List<S> source, List<T> result, Func<S, T> selector, Func<T, bool> predicate)
        {
            for (var i = 0; i < source.Count; ++i)
            {
                var input = selector(source[i]);
                if (predicate(input))
                {
                    result.Add(input);
                }
            }
        }

    }

    sealed class ToList<T>
        : Consumer<T, List<T>>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        , Optimizations.IPipelineList<T>
    {
        public ToList() : base(new List<T>()) { }

        public ToList(int count) : base(new List<T>(count)) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }

        public ChainStatus Select<S>(List<S> source, Func<S, T> selector)
        {
            ToListImpl.Select(source, Result, selector);
            return ChainStatus.Filter;
        }

        public ChainStatus SelectWhere<S>(List<S> source, Func<S, T> selector, Func<T, bool> predicate)
        {
            ToListImpl.SelectWhere(source, Result, selector, predicate);
            return ChainStatus.Filter;
        }

        public ChainStatus Where(List<T> source, Func<T, bool> predicate)
        {
            ToListImpl.Where(source, Result, predicate);
            return ChainStatus.Filter;
        }

        public ChainStatus WhereSelect<S>(List<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            ToListImpl.WhereSelect(source, Result, predicate, selector);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            ToListImpl.Execute(source, Result);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            ToListImpl.Execute<T, Enumerable, Enumerator>(source, Result);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            ToListImpl.Select(source, Result, selector);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            ToListImpl.Select<S, T, Enumerable, Enumerator>(source, Result, selector);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            ToListImpl.SelectMany(source, Result, span, resultSelector);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            ToListImpl.Where(source, Result, predicate);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            ToListImpl.Where<T, Enumerable, Enumerator>(source, Result, predicate);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            ToListImpl.WhereSelect(source, Result, predicate, selector);
            return ChainStatus.Filter;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            ToListImpl.WhereSelect<S, T, Enumerable, Enumerator>(source, Result, predicate, selector);
            return ChainStatus.Filter;
        }
    }
}
