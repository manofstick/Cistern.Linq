using Cistern.Linq.Utils;
using Microsoft.FSharp.Collections;
using System;

namespace Cistern.Linq.Consumer
{
    internal sealed class ToFSharpList<T>
        : Consumer<T, FSharpList<T>>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
    {
        FSharpListBuilder<T> builder;

        public ToFSharpList() : base(null) =>
            builder = new FSharpListBuilder<T>(true);

        public override ChainStatus ProcessNext(T input)
        {
            builder.Add(input);
            return ChainStatus.Flow;
        }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Result = builder.ToFSharpList();

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            foreach (var item in source)
            {
                builder.Add(item);
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
            {
                builder.Add(item);
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                builder.Add(selector(item));
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                builder.Add(selector(item));
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var item in span)
            {
                builder.Add(resultSelector(source, item));
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
            return ChainStatus.Flow;
        }
    }
}
