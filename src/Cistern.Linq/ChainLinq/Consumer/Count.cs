using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class Count<T, TCount, Accumulator, Quotient, Maths>
        : Consumer<T, Accumulator>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        where TCount : struct
        where Accumulator : struct
        where Quotient : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<TCount, Accumulator, Quotient>
    {
        public Count() : base(default(Maths).Zero) {}

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            Maths maths = default;

            Result = maths.AddInt(Result, source.Length);

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            var tryLength = source.TryLength;
            if (tryLength.HasValue)
            {
                Result = maths.Add(Result, maths.Cast(tryLength.Value));
            }
            else
            {
                var result = Result;
                foreach (var input in source)
                {
                    result = maths.Add(result, maths.One);
                }
                Result = result;
            }

            return ChainStatus.Flow;
        }

        public override ChainStatus ProcessNext(T input)
        {
            checked
            {
                Maths maths = default;

                Result = maths.Add(Result, maths.One);
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            // really this should have be "Result += source.Length", but someone decided that we want selector side effects...
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                var forTheSideEffect = selector(input);
                result = maths.Add(result, maths.One);
            }
            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            // really this should have be "Result += source.Length", but someone decided that we want selector side effects...
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                var forTheSideEffect = selector(input);
                result = maths.Add(result, maths.One);
            }
            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result = maths.Add(result, maths.One);
                }
            }
            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    result = maths.Add(result, maths.One);
                }
            }
            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            // really this should have be "Result += source.Length", but someone decided that we want selector side effects...
            Maths maths = default;

            var result = Result;
            foreach (var input in span)
            {
                var forTheSideEffect = resultSelector(source, input);
                result = maths.Add(result, maths.One);
            }
            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var forTheSideEffect = selector(input);
                    result = maths.Add(result, maths.One);
                }
            }
            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            var result = Result;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var forTheSideEffect = selector(input);
                    result = maths.Add(result, maths.One);
                }
            }
            Result = result;
            return ChainStatus.Flow;
        }
    }
}
