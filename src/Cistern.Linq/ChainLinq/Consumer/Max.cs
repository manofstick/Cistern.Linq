using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cistern.Linq.ChainLinq.Consumer
{
    abstract class MaxGeneric<T, Accumulator, Maths>
        : Consumer<T, T>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        where T : struct
        where Accumulator : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator>
    {
        protected bool _noData;

        public MaxGeneric() : base(default(Maths).MaxInit) =>
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
            for (; maths.IsNaN(result) && idx < source.Length; ++idx)
            {
                result = source[idx];
            }
            for(; idx < source.Length; ++idx)
            {
                var input = source[idx];
                if (maths.GreaterThan(input, result))
                    result = input;
            }

            Result = result;
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var t in source)
            {
                noData = false;
                if (maths.GreaterThan(t, result) || maths.IsNaN(result))
                    result = t;
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
                if (maths.GreaterThan(t, result) || maths.IsNaN(result))
                    result = t;
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
                if (maths.GreaterThan(t, result) || maths.IsNaN(result))
                    result = t;
            }

            Result = result;

            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    noData = false;
                    if (maths.GreaterThan(input, result) || maths.IsNaN(result))
                        result = input;
                }
            }

            _noData = noData;
            Result = result;
        }
        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            Maths maths = default;

            var noData = _noData;
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    noData = false;
                    if (maths.GreaterThan(input, result) || maths.IsNaN(result))
                        result = input;
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
                    if (maths.GreaterThan(t, result) || maths.IsNaN(result))
                        result = t;
                }
            }

            _noData = noData;
            Result = result;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T> selector)
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
                    if (maths.GreaterThan(t, result) || maths.IsNaN(result))
                        result = t;
                }
            }

            _noData = noData;
            Result = result;
        }

    }

    interface ILogic<T>
        where T : struct
    {
        void Init(T? result);
        bool Process(T? input);
        T? Result { get; }
    }

    struct MaxNullableLogic<T, Accumulator, Maths> : ILogic<T>
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

                Result = maths.MaxInit;
            }

            if (input.HasValue)
            {
                var i = input.GetValueOrDefault();
                var r = Result.GetValueOrDefault();
                if (maths.GreaterThan(i, r) || maths.IsNaN(r))
                {
                    Result = i;
                }
            }

            return true;
        }
    }

    sealed class GenericNullable<T, Logic>
        : Consumer<T?, T?>
        , Optimizations.IHeadStart<T?>
        , Optimizations.ITailEnd<T?>
        where T : struct
        where Logic : ILogic<T>
    {
        public GenericNullable() : base(null) { }

        public override ChainStatus ProcessNext(T? input)
        {
            Logic logic = default; logic.Init(Result);
            var status = logic.Process(input) ? ChainStatus.Flow : ChainStatus.Stop;
            Result = logic.Result;

            return status;
        }

        void Optimizations.IHeadStart<T?>.Execute(ReadOnlySpan<T?> source)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (!logic.Process(input))
                    break;
            }

            Result = logic.Result;
        }

        void Optimizations.IHeadStart<T?>.Execute<Enumerator>(Optimizations.ITypedEnumerable<T?, Enumerator> source)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (!logic.Process(input))
                    break;
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.Select<S>(ReadOnlySpan<S> source, Func<S, T?> selector)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (!logic.Process(selector(input)))
                    break;
            }

            Result = logic.Result;
        }

        ChainStatus Optimizations.ITailEnd<T?>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T?> resultSelector)
        {
            Logic logic = default; logic.Init(Result);
            ChainStatus status = ChainStatus.Flow;

            foreach (var input in span)
            {
                if (!logic.Process(resultSelector(source, input)))
                {
                    status = ChainStatus.Stop;
                    break;
                }
            }

            Result = logic.Result;

            return status;
        }

        void Optimizations.ITailEnd<T?>.Where(ReadOnlySpan<T?> source, Func<T?, bool> predicate)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    if (!logic.Process(input))
                        break;
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.Where<Enumerator>(Optimizations.ITypedEnumerable<T?, Enumerator> source, Func<T?, bool> predicate)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    if (!logic.Process(input))
                        break;
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    if (!logic.Process(selector(input)))
                        break;
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            Logic logic = default; logic.Init(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    if (!logic.Process(selector(input)))
                        break;
                }
            }

            Result = logic.Result;
        }
    }

    sealed class MaxInt : MaxGeneric<int, int, Maths.OpsInt>
    {
        public override ChainStatus ProcessNext(int input)
        {
            _noData = false;
            if (input > Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxLong : MaxGeneric<long, long, Maths.OpsLong>
    {
        public override ChainStatus ProcessNext(long input)
        {
            _noData = false;
            if (input > Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxDouble : MaxGeneric<double, double, Maths.OpsDouble>
    {
        public override ChainStatus ProcessNext(double input)
        {
            _noData = false;
            if (input > Result || double.IsNaN(Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxFloat : MaxGeneric<float, double, Maths.OpsFloat>
    {
        public override ChainStatus ProcessNext(float input)
        {
            _noData = false;
            if (input > Result || float.IsNaN(Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxDecimal : MaxGeneric<decimal, decimal, Maths.OpsDecimal>
    {
        public override ChainStatus ProcessNext(decimal input)
        {
            _noData = false;
            if (input > Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxValueType<T> : Consumer<T, T>
    {
        bool _first;

        public MaxValueType() : base(default) =>
            _first = true;

        public override ChainStatus ProcessNext(T input)
        {
            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) > 0)
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

    sealed class MaxRefType<T> : Consumer<T, T>
    {
        public MaxRefType() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) > 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }
}
