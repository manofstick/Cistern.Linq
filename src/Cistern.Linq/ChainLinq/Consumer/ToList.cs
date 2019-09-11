using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
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

        private void EnsureCapacity(int length)
        {
            Result.Capacity = Result.Count + length;
        }

        private void EnsureCapacity(int? maybeLength)
        {
            if (maybeLength.HasValue)
            {
                Result.Capacity = Result.Count + maybeLength.Value;
            }
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                result.Add(input);
            }

            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            var result = Result;

            EnsureCapacity(source.TryLength);
            foreach (var input in source)
            {
                result.Add(input);
            }

            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                result.Add(selector(input));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            var result = Result;

            var maybeLength = source.TryLength;
            if (maybeLength.HasValue)
                EnsureCapacity(maybeLength.Value);

            foreach (var input in source)
            {
                result.Add(selector(input));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            var result = Result;

            EnsureCapacity(span.Length);
            foreach (var input in span)
            {
                result.Add(resultSelector(source, input));
            }

            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(input);
                }
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(input);
                }
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(selector(input));
                }
            }

            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result.Add(selector(input));
                }
            }

            Result = result;
            return ChainStatus.Flow;
        }
    }
}
