using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class ToList<T>
        : Consumer<T, List<T>>
        , Optimizations.ITailEnd<T>
    {
        public ToList() : base(new List<T>()) { }

        public ToList(int count) : base(new List<T>(count)) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                Result.Add(selector(input));
            }
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var input in span)
            {
                Result.Add(resultSelector(source, input));
            }
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach(var input in source)
            {
                if (predicate(input))
                {
                    Result.Add(input);
                }
            }
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    Result.Add(input);
                }
            }
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    Result.Add(selector(input));
                }
            }
        }
    }
}
