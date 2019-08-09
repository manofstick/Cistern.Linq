using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    abstract class SumGeneric<T, Maths>
        : Consumer<T, T>
        , Optimizations.IWhereArray<T>
        , Optimizations.ISelectMany<T>
        , Optimizations.IPipeline<ReadOnlyMemory<T>>
        , Optimizations.IPipeline<List<T>>
        , Optimizations.IPipeline<IEnumerable<T>>
        where T : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, T>
    {
        public SumGeneric() : base(default(Maths).Zero) { }

        public void Pipeline(ReadOnlyMemory<T> source)
        {
            Maths maths = default;

            T sum = Result;
            foreach (var x in source.Span)
            {
                sum = maths.Add(sum, x);
            }

            Result = sum;
        }
        public void Pipeline(List<T> source)
        {
            Maths maths = default;

            T sum = Result;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            Result = sum;
        }

        public void Pipeline(IEnumerable<T> source)
        {
            Maths maths = default;

            T sum = Result;
            foreach (var x in source)
            {
                sum = maths.Add(sum, x);
            }

            Result = sum;
        }

        public void Where(T[] memory, Func<T, bool> predicate)
        {
            Maths maths = default;

            T sum = Result;
            foreach (var x in memory)
            {
                if (predicate(x))
                    sum = maths.Add(sum, x);
            }

            Result = sum;
        }

        public ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            Maths maths = default;

            var sum = Result;
            foreach (var item in span)
            {
                sum = maths.Add(sum, resultSelector(source, item));
            }
            Result = sum;

            return ChainStatus.Flow;
        }
    }

    sealed class SumDouble : SumGeneric<double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumInt : SumGeneric<int, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumLong : SumGeneric<long, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }
    sealed class SumDecimal : SumGeneric<Decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(Decimal input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }

    // TODO: below....

    sealed class SumNullableInt : Consumer<int?, int?>
    {
        public SumNullableInt() : base(0) { }

        public override ChainStatus ProcessNext(int? input)
        {
            checked
            {
                Result += input.GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableLong : Consumer<long?, long?>
    {
        public SumNullableLong() : base(0L) { }

        public override ChainStatus ProcessNext(long? input)
        {
            checked
            {
                Result += input.GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }


    sealed class SumFloat : Consumer<float, float>
    {
        double _sum = 0.0;

        public SumFloat() : base(default) { }

        public override ChainStatus ProcessNext(float input)
        {
            _sum += input;
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }

    sealed class SumNullableFloat : Consumer<float?, float?>
    {
        double _sum = 0.0;

        public SumNullableFloat() : base(default) { }

        public override ChainStatus ProcessNext(float? input)
        {
            _sum += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }


    sealed class SumNullableDouble : Consumer<double?, double?>
    {
        public SumNullableDouble() : base(0.0) { }

        public override ChainStatus ProcessNext(double? input)
        {
            Result += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDecimal : Consumer<decimal?, decimal?>
    {
        public SumNullableDecimal() : base(0M) { }

        public override ChainStatus ProcessNext(decimal? input)
        {
            Result += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }
}
