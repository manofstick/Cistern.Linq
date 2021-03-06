﻿using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{
    abstract class SumGeneric<T, Accumulator, Quotient, Maths>
        : Consumer<T, T>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        where T : struct
        where Accumulator : struct
        where Quotient : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator, Quotient>
    {
        protected Accumulator accumulator = default(Maths).Zero;

        public SumGeneric() : base(default) { }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Result = default(Maths).Cast(accumulator);

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> memory, Func<T, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                if (predicate(x))
                    sum = maths.Add(sum, x);
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, x);
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> memory, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable memory, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var item in span)
            {
                sum = maths.Add(sum, resultSelector(source, item));
            }
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }
    }

    abstract class SumGenericNullable<T, Accumulator, Quotient, Maths>
        : Consumer<T?, T>
        , Optimizations.IHeadStart<T?>
        , Optimizations.ITailEnd<T?>
        where T : struct
        where Accumulator : struct
        where Quotient : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator, Quotient>
    {
        protected Accumulator accumulator = default(Maths).Zero;

        public SumGenericNullable() : base(default) { }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Result = default(Maths).Cast(accumulator);

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T?>.Execute(ReadOnlySpan<T?> source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T?>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Where(ReadOnlySpan<T?> memory, Func<T?, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                if (predicate(x))
                    sum = maths.Add(sum, x);
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Where<Enumerable, Enumerator>(Enumerable source, Func<T?, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, x);
            }

            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Select<S>(ReadOnlySpan<S> memory, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Select<Enumerable, Enumerator, S>(Enumerable memory, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in memory)
            {
                sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T?> resultSelector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var item in span)
            {
                sum = maths.Add(sum, resultSelector(source, item));
            }
            accumulator = sum;

            return ChainStatus.Flow;
        }
        ChainStatus Optimizations.ITailEnd<T?>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            foreach (var x in source)
            {
                if (predicate(x))
                    sum = maths.Add(sum, selector(x));
            }

            accumulator = sum;
            return ChainStatus.Flow;
        }
    }

    sealed class SumDouble : SumGeneric<double, double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double input)
        {
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDouble : SumGenericNullable<double, double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double? input)
        {
            accumulator += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }

    sealed class SumFloat : SumGeneric<float, double, float, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float input)
        {
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableFloat : SumGenericNullable<float, double, float, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float? input)
        {
            accumulator += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }

    sealed class SumInt : SumGeneric<int, int, double, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int input)
        {
            checked
            {
                accumulator += input;
                return ChainStatus.Flow;
            }
        }
    }

    sealed class SumNullableInt : SumGenericNullable<int, int, double, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int? input)
        {
            checked
            {
                accumulator += input.GetValueOrDefault();
                return ChainStatus.Flow;
            }
        }
    }

    sealed class SumLong : SumGeneric<long, long, double, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long input)
        {
            checked
            {
                accumulator += input;
                return ChainStatus.Flow;
            }
        }
    }

    sealed class SumNullableLong : SumGenericNullable<long, long, double, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long? input)
        {
            checked
            {
                accumulator += input.GetValueOrDefault();
                return ChainStatus.Flow;
            }
        }
    }

    sealed class SumDecimal : SumGeneric<decimal, decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal input)
        {
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDecimal : SumGenericNullable<decimal, decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal? input)
        {
            accumulator += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }
}
