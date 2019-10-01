using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{
    abstract class AverageGeneric<T, Accumulator, Quotient, Maths>
        : Consumer<T, Quotient>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        where T : struct
        where Accumulator : struct
        where Quotient : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator, Quotient>
    {
        protected Accumulator accumulator = default(Maths).Zero;
        protected long counter = 0;

        public AverageGeneric() : base(default) { }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            if (counter == 0)
            {
                ThrowHelper.ThrowNoElementsException();
            }
            Result = default(Maths).DivLong(accumulator, counter);

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                count++;
                sum = maths.Add(sum, x);
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                count++;
                sum = maths.Add(sum, x);
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> memory, Func<T, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in memory)
            {
                if (predicate(x))
                {
                    count++;
                    sum = maths.Add(sum, x);
                }
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (predicate(x))
                {
                    count++;
                    sum = maths.Add(sum, x);
                }
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> memory, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in memory)
            {
                count++;
                sum = maths.Add(sum, selector(x));
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable memory, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in memory)
            {
                count++;
                sum = maths.Add(sum, selector(x));
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var item in span)
            {
                count++;
                sum = maths.Add(sum, resultSelector(source, item));
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (predicate(x))
                {
                    count++;
                    sum = maths.Add(sum, selector(x));
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (predicate(x))
                {
                    count++;
                    sum = maths.Add(sum, selector(x));
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }
    }

    abstract class AverageGenericNullable<T, Accumulator, Quotient, Maths>
        : Consumer<T?, Quotient?>
        , Optimizations.IHeadStart<T?>
        , Optimizations.ITailEnd<T?>
        where T : struct
        where Accumulator : struct
        where Quotient : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator, Quotient>
    {
        protected Accumulator accumulator = default(Maths).Zero;
        protected long counter;

        public AverageGenericNullable() : base(default) { }

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            if (counter > 0)
            {
                Result = default(Maths).DivLong(accumulator, counter);
            }

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T?>.Execute(ReadOnlySpan<T?> source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (x.HasValue)
                {
                    count++;
                    sum = maths.Add(sum, x.Value);
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T?>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (x.HasValue)
                {
                    count++;
                    sum = maths.Add(sum, x.Value);
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Where(ReadOnlySpan<T?> memory, Func<T?, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in memory)
            {
                if (predicate(x))
                {
                    if (x.HasValue)
                    {
                        count++;
                        sum = maths.Add(sum, x.Value);
                    }
                }
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Where<Enumerable, Enumerator>(Enumerable source, Func<T?, bool> predicate)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var x in source)
            {
                if (predicate(x))
                {
                    if (x.HasValue)
                    {
                        count++;
                        sum = maths.Add(sum, x.Value);
                    }
                }
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Select<S>(ReadOnlySpan<S> memory, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var input in memory)
            {
                var x = selector(input);
                if (x.HasValue)
                {
                    count++;
                    sum = maths.Add(sum, x.Value);
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.Select<Enumerable, Enumerator, S>(Enumerable memory, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var input in memory)
            {
                var x = selector(input);
                if (x.HasValue)
                {
                    count++;
                    sum = maths.Add(sum, x.Value);
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T?> resultSelector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var item in span)
            {
                var x = resultSelector(source, item);
                if (x.HasValue)
                {
                    count++;
                    sum = maths.Add(sum, x.Value);
                }
            }
            counter = count;
            accumulator = sum;

            return ChainStatus.Flow;
        }
        ChainStatus Optimizations.ITailEnd<T?>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var x = selector(input);
                    if (x.HasValue)
                    {
                        count++;
                        sum = maths.Add(sum, x.Value);
                    }
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T?>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Maths maths = default;

            Accumulator sum = accumulator;
            var count = counter;
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var x = selector(input);
                    if (x.HasValue)
                    {
                        count++;
                        sum = maths.Add(sum, x.Value);
                    }
                }
            }
            counter = count;
            accumulator = sum;
            return ChainStatus.Flow;
        }
    }

    sealed class AverageDouble : AverageGeneric<double, double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double input)
        {
            counter++;
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class AverageNullableDouble : AverageGenericNullable<double, double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double? input)
        {
            if (input.HasValue)
            {
                counter++;
                accumulator += input.Value;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class AverageFloat : AverageGeneric<float, double, float, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float input)
        {
            counter++;
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class AverageNullableFloat : AverageGenericNullable<float, double, float, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float? input)
        {
            if (input.HasValue)
            {
                counter++;
                accumulator += input.Value;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class AverageInt : AverageGeneric<int, int, double, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int input)
        {
            checked
            {
                counter++;
                accumulator += input;
                return ChainStatus.Flow;
            }
        }
    }

    sealed class AverageNullableInt : AverageGenericNullable<int, int, double, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int? input)
        {
            checked
            {
                if (input.HasValue)
                {
                    counter++;
                    accumulator += input.Value;
                }
                return ChainStatus.Flow;
            }
        }
    }

    sealed class AverageLong : AverageGeneric<long, long, double, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long input)
        {
            checked
            {
                counter++;
                accumulator += input;
                return ChainStatus.Flow;
            }
        }
    }

    sealed class AverageNullableLong : AverageGenericNullable<long, long, double, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long? input)
        {
            checked
            {
                if (input.HasValue)
                {
                    counter++;
                    accumulator += input.Value;
                }
                return ChainStatus.Flow;
            }
        }
    }

    sealed class AverageDecimal : AverageGeneric<decimal, decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal input)
        {
            counter++;
            accumulator += input;
            return ChainStatus.Flow;
        }
    }

    sealed class AverageNullableDecimal : AverageGenericNullable<decimal, decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal? input)
        {
            if (input.HasValue)
            {
                counter++;
                accumulator += input.Value;
            }
            return ChainStatus.Flow;
        }
    }
}
