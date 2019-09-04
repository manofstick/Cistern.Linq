using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cistern.Linq.ChainLinq.Consumer
{
    abstract class MinGeneric<T, Accumulator, Maths>
        : Consumer<T, T>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        where T : struct
        where Accumulator : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator>
    {
        protected bool _noData;

        public MinGeneric() : base(default(Maths).MinInit) =>
            _noData = true;

        public override void ChainComplete()
        {
            if (_noData)
            {
                ThrowHelper.ThrowNoElementsException();
            }
        }

        void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            Maths maths = default;

            var result = Result;

            _noData &= source.Length == 0;

            var idx = 0;

            const int NumberOfVectorsToMakeThisWorthwhile = 0; // from some random testing
            if (maths.SupportsVectorization && ((source.Length - idx) / Vector<T>.Count > NumberOfVectorsToMakeThisWorthwhile))
            {
                var asVector = MemoryMarshal.Cast<T, Vector<T>>(source);
                var mins = new Vector<T>(result);
                if (maths.HasNaNs)
                {
                    var nan = new Vector<T>(maths.NaN);
                    foreach (var v in asVector)
                    {
                        if (Vector.EqualsAny(Vector.Xor(v, nan), Vector<T>.Zero))
                        {
                            Result = maths.NaN;
                            return;
                        }
                        mins = Vector.Min(mins, v);
                    }
                }
                else
                {
                    foreach (var v in asVector)
                    {
                        mins = Vector.Min(mins, v);
                    }
                }
                for (var i = 0; i < Vector<T>.Count; ++i)
                {
                    var input = mins[i];
                    if (maths.LessThan(input, result))
                        result = input;
                }

                idx += asVector.Length * Vector<T>.Count;
            }

            for (; idx < source.Length; ++idx)
            {
                var input = source[idx];
                if (maths.LessThan(input, result)) 
                    result = input;
                else if (maths.IsNaN(input))
                {
                    result = input;
                    break;
                }
            }

            Result = result;
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var t in source)
            {
                noData = false;
                if (maths.LessThan(t, result))
                    result = t;
                else if (maths.IsNaN(t))
                {
                    result = t;
                    break;
                }
            }

            _noData = noData;
            Result = result;
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            Maths maths = default;

            var result = Result;

            _noData &= source.Length == 0;
            foreach (var s in source)
            {
                var t = selector(s);
                if (maths.LessThan(t, result))
                    result = t;
                else if (maths.IsNaN(t))
                {
                    result = t;
                    break;
                }
            }

            Result = result;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            Maths maths = default;

            var result = Result;

            _noData &= span.Length == 0;
            foreach (var s in span)
            {
                var t = resultSelector(source, s);
                if (maths.LessThan(t, result))
                    result = t;
                else if (maths.IsNaN(t))
                {
                    result = t;
                    break;
                }
            }

            Result = result;

            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var t in source)
            {
                if (predicate(t))
                {
                    noData = false;
                    if (maths.LessThan(t, result))
                        result = t;
                    else if (maths.IsNaN(t))
                    {
                        result = t;
                        break;
                    }
                }
            }

            _noData = noData;
            Result = result;
        }
        void Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var t in source)
            {
                if (predicate(t))
                {
                    noData = false;
                    if (maths.LessThan(t, result))
                        result = t;
                    else if (maths.IsNaN(t))
                    {
                        result = t;
                        break;
                    }
                }
            }

            _noData = noData;
            Result = result;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var s in source)
            {
                if (predicate(s))
                {
                    noData = false;
                    var t = selector(s);
                    if (maths.LessThan(t, result))
                        result = t;
                    else if (maths.IsNaN(t))
                    {
                        result = t;
                        break;
                    }
                }
            }

            _noData = noData;
            Result = result;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var s in source)
            {
                if (predicate(s))
                {
                    noData = false;
                    var t = selector(s);
                    if (maths.LessThan(t, result))
                        result = t;
                    else if (maths.IsNaN(t))
                    {
                        result = t;
                        break;
                    }
                }
            }

            _noData = noData;
            Result = result;
        }
    }

    sealed class MinInt : MinGeneric<int, int, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int input)
        {
            _noData = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }
    sealed class MinLong : MinGeneric<long, long, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long input)
        {
            _noData = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MinFloat : MinGeneric<float, double, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float input)
        {
            _noData = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (float.IsNaN(input))
            {
                Result = float.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MinDouble : MinGeneric<double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double input)
        {
            _noData = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (double.IsNaN(input))
            {
                Result = double.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MinDecimal : MinGeneric<decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal input)
        {
            _noData = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    struct MinNullableLogic<T, Accumulator, Maths> : INullableGenericLogic<T>
        where T : struct
        where Accumulator : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator>
    {
        public T? Result { get; private set; }

        public void Init(T? result)
        {
            this.Result = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Process(T? input)
        {
            var maths = default(Maths);

            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return true;
                }

                Result = maths.MinInit;
            }

            if (input.HasValue)
            {
                var i = input.GetValueOrDefault();
                if (maths.LessThan(i, Result.GetValueOrDefault()))
                {
                    Result = i;
                }
                else if (maths.IsNaN(i))
                {
                    Result = i;
                    return false;
                }
            }

            return true;
        }
    }

    sealed class MinValueType<T> : Consumer<T, T>
    {
        bool _first;

        public MinValueType() : base(default) =>
            _first = true;

        public override ChainStatus ProcessNext(T input)
        {
            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) < 0)
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                ThrowHelper.ThrowNoElementsException();
            }
        }
    }

    sealed class MinRefType<T> : Consumer<T, T>
    {
        public MinRefType() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) < 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }
}
