using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class Aggregate<T, TAccumulate, TResult>
        : Consumer<T, TResult>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>

    {
        readonly Func<TAccumulate, T, TAccumulate> _func;
        readonly Func<TAccumulate, TResult> _resultSelector;
        
        TAccumulate _accumulate;

        public Aggregate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) : base(default) =>
            (_accumulate, _func, _resultSelector) = (seed, func, resultSelector);

        public override ChainStatus ProcessNext(T input)
        {
            _accumulate = _func(_accumulate, input);

            return ChainStatus.Flow;
        }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Result = _resultSelector(_accumulate);

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            var accumulate = _accumulate;

            foreach(var input in source)
                accumulate = _func(accumulate, input);

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
                accumulate = _func(accumulate, input);

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
                accumulate = _func(accumulate, selector(input));

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
                accumulate = _func(accumulate, selector(input));

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    accumulate = _func(accumulate, input);
            }

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    accumulate = _func(accumulate, input);
            }

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            var accumulate = _accumulate;

            foreach (var input in span)
            {
                accumulate = _func(accumulate, resultSelector(source, input));
            }

            _accumulate = accumulate;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    accumulate = _func(accumulate, selector(input));
            }

            _accumulate = accumulate;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            var accumulate = _accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    accumulate = _func(accumulate, selector(input));
            }

            _accumulate = accumulate;
            return ChainStatus.Flow;
        }
    }
}
