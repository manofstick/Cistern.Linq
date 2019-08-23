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

        void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                result.Add(input);
            }

            Result = result;
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source)
        {
            var result = Result;

            EnsureCapacity(source.TryLength);
            foreach (var input in source)
            {
                result.Add(input);
            }

            Result = result;
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                result.Add(selector(input));
            }

            Result = result;
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

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
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
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
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
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
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
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T> selector)
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
        }
    }
}
