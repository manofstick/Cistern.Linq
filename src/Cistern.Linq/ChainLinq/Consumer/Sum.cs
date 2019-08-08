using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Cistern.Linq.ChainLinq.Consumer
{
    interface ISelectMany<T>
    {
        ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector);
    }

    static class SumHelper
    {
#if SIMPLE_SUM
        public static double Sum(Span<double> data, Func<double, bool> predicate)
        {
            double sum = 0;
            foreach(var x in data)
            {
                if (predicate(x))
                    sum += x;
            }
            return sum;
        }
#else
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        struct Bool8
        {
            public bool _0;
            public bool _1;
            public bool _2;
            public bool _3;
            public bool _4;
            public bool _5;
            public bool _6;
            public bool _7;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        struct Byte8
        {
            public byte _0;
            public byte _1;
            public byte _2;
            public byte _3;
            public byte _4;
            public byte _5;
            public byte _6;
            public byte _7;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        struct Double8
        {
            public double _0;
            public double _1;
            public double _2;
            public double _3;
            public double _4;
            public double _5;
            public double _6;
            public double _7;
        }

        // This Sum has slightly worse best-case performance (i.e. where the where condition is always true)
        // but it's performance is consistent regardless of predicate.
        public static double Sum(Span<double> data, Func<double, bool> predicate)
        {
            // We can't rely on predicate returning 0/1 for the bool, as the CLI specification
            // only specifies that 0=false (and any other 8-bit pattern is true), so we use a "!" on bool to
            // ensure 0/1 value in bool slot so that we can then rely on the cast to byte.
            Span<Bool8> bools = stackalloc Bool8[1];
            Span<Byte8> bytes = MemoryMarshal.Cast<Bool8, Byte8>(bools);

            return Summer(data, predicate, ref bools[0], ref bytes[0]);
        }

        static double Summer(Span<double> data, Func<double, bool> predicate, ref Bool8 bools, ref Byte8 bytes)
        {
            // An implementation of Sum that avoids branching logic by summing all elements multipled by 0 if excluded
            // or 1 if included. NaNs are handled gracefully (with NaN result handled with short-circuit).
            // 
            // NB: Uses some trickery to cast bool to byte ==> ((byte)(!bool) ^ (int)1) should ensure false=0, true=1
            double sum = 0.0;

            var data8 = MemoryMarshal.Cast<double, Double8>(data);
            var i8 = 0; // index in data8
            for (; i8 < data8.Length; ++i8)
            {
                var tmp = sum;

                // Relying on implemenation detail of the "boolean not (!)" such that it stores a 0 or a 1 in the bool slot.
                // Current c# implementation generates the following code, where I believe it is exceedingly unlikely that 
                // this would change such that values other that 0 or 1 would be stored (although inline IL to enforce
                // would be nice...)
                //
                // <<result of predicate on top of stack. i.e. any bit pattern from 0b0000_0000 to 0b1111_1111>>
                // ldc.i4.0
                // ceq
                // stfld <<bools._{x}>>
                //
                // where "ceq" states (https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ceq):
                // Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack;
                // otherwise 0 (int32) is pushed onto the evaluation stack.
                bools._0 = !predicate(data8[i8]._0);
                bools._1 = !predicate(data8[i8]._1);
                bools._2 = !predicate(data8[i8]._2);
                bools._3 = !predicate(data8[i8]._3);
                bools._4 = !predicate(data8[i8]._4);
                bools._5 = !predicate(data8[i8]._5);
                bools._6 = !predicate(data8[i8]._6);
                bools._7 = !predicate(data8[i8]._7);

                // the xor here is used to invert the boolean bit which was stored in << bools._{x} >>
                // we store the sum in a temporary variable as we have to deal with the possibility that NaN will
                // be introduced as "0 * ((Postive|Negative)Infinity|NaN) == NaN", so fall-back to branching loop
                tmp += (bytes._0 ^ 1) * data8[i8]._0;
                tmp += (bytes._1 ^ 1) * data8[i8]._1;
                tmp += (bytes._2 ^ 1) * data8[i8]._2;
                tmp += (bytes._3 ^ 1) * data8[i8]._3;
                tmp += (bytes._4 ^ 1) * data8[i8]._4;
                tmp += (bytes._5 ^ 1) * data8[i8]._5;
                tmp += (bytes._6 ^ 1) * data8[i8]._6;
                tmp += (bytes._7 ^ 1) * data8[i8]._7;
                if (Double.IsNaN(tmp))
                    goto foundNaN;

                sum = tmp;
            }
            goto remainingElements;

            foundNaN:
            if (!bools._0) sum += data8[i8]._0;
            if (!bools._1) sum += data8[i8]._1;
            if (!bools._2) sum += data8[i8]._2;
            if (!bools._3) sum += data8[i8]._3;
            if (!bools._4) sum += data8[i8]._4;
            if (!bools._5) sum += data8[i8]._5;
            if (!bools._6) sum += data8[i8]._6;
            if (!bools._7) sum += data8[i8]._7;
            if (Double.IsNaN(sum))
                goto done;
            ++i8;

            remainingElements:
            // handle the remaining fields either at tail of collection or after NaN
            for (var i = i8 * 8; i < data.Length; ++i)
            {
                var x = data[i];
                if (predicate(x))
                {
                    sum += x;
                }
            }

            done:
            return sum;
        }
#endif
    }

    sealed class SumInt
        : Consumer<int, int>
        , Optimizations.IPipeline<ReadOnlyMemory<int>>
        , Optimizations.IPipeline<List<int>>
        , Optimizations.IPipeline<IEnumerable<int>>
    {
        public SumInt() : base(0) { }

        public void Pipeline(ReadOnlyMemory<int> source)
        {
            checked
            {
                int sum = 0;
                foreach (var x in source.Span)
                {
                    sum += x;
                }
                Result = sum;
            }
        }

        public void Pipeline(List<int> source)
        {
            checked
            {
                int sum = 0;
                foreach (var x in source)
                {
                    sum += x;
                }
                Result = sum;
            }
        }

        public void Pipeline(IEnumerable<int> source)
        {
            checked
            {
                int sum = 0;
                foreach (var x in source)
                {
                    sum += x;
                }
                Result = sum;
            }
        }

        public override ChainStatus ProcessNext(int input)
        {
            checked
            {
                Result += input;
            }
            return ChainStatus.Flow;
        }
    }

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

    sealed class SumLong
        : Consumer<long, long>
        , Optimizations.IPipeline<ReadOnlyMemory<long>>
        , Optimizations.IPipeline<List<long>>
        , Optimizations.IPipeline<IEnumerable<long>>
    {
        public SumLong() : base(0L) { }

        public void Pipeline(ReadOnlyMemory<long> source)
        {
            checked
            {
                long sum = 0;
                foreach (var x in source.Span)
                {
                    sum += x;
                }
                Result = sum;
            }
        }
        public void Pipeline(List<long> source)
        {
            checked
            {
                long sum = 0;
                foreach (var x in source)
                {
                    sum += x;
                }
                Result = sum;
            }
        }
        public void Pipeline(IEnumerable<long> source)
        {
            checked
            {
                long sum = 0;
                foreach (var x in source)
                {
                    sum += x;
                }
                Result = sum;
            }
        }

        public override ChainStatus ProcessNext(long input)
        {
            checked
            {
                Result += input;
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

    sealed class SumDouble 
        : Consumer<double, double>
        , Optimizations.IWhereArray
        , ISelectMany<double>
        , Optimizations.IPipeline<ReadOnlyMemory<double>>
        , Optimizations.IPipeline<List<double>>
        , Optimizations.IPipeline<IEnumerable<double>>
    {
        public SumDouble() : base(0.0) { }


        public override ChainStatus ProcessNext(double input)
        {
            Result += input;
            return ChainStatus.Flow;
        }

        public void Pipeline(ReadOnlyMemory<double> source)
        {
            double sum = 0.0;
            foreach (var x in source.Span)
            {
                sum += x;
            }
            Result = sum;
        }
        public void Pipeline(List<double> source)
        {
            double sum = 0.0;
            foreach (var x in source)
            {
                sum += x;
            }
            Result = sum;
        }

        public void Pipeline(IEnumerable<double> source)
        {
            double sum = 0.0;
            foreach (var x in source)
            {
                sum += x;
            }
            Result = sum;
        }

        public void Where<T>(T[] memory, Func<T, bool> predicate) =>
            Result = SumHelper.Sum((double[])(object)memory, (Func<double, bool>)(object)predicate);

        public ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, double> resultSelector)
        {
            var sum = Result;
            foreach (var item in span)
            {
                sum += resultSelector(source, item);
            }
            Result = sum;
            return ChainStatus.Flow;
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

    sealed class SumDecimal : Consumer<decimal, decimal>
    {
        public SumDecimal() : base(0M) { }

        public override ChainStatus ProcessNext(decimal input)
        {
            Result += input;
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
