﻿using System;
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
            foreach (var input in source)
            {
                if (maths.GreaterThan(input, result) || maths.IsNaN(result))
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

    sealed class MaxGenericNullable<T, Accumulator, Maths>
        : Consumer<T?, T?>
        , Optimizations.IHeadStart<T?>
        , Optimizations.ITailEnd<T?>
        where T : struct
        where Accumulator : struct
        where Maths : struct, Cistern.Linq.Maths.IMathsOperations<T, Accumulator>
    {
        public MaxGenericNullable() : base(null) { }

        struct Logic
        {
            public T? Result;

            public Logic(T? result)
            {
                this.Result = result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Process(T? input)
            {
                if (!Result.HasValue)
                {
                    if (!input.HasValue)
                    {
                        return;
                    }

                    Result = default(Maths).MaxInit;
                }

                if (input.HasValue)
                {
                    var i = input.GetValueOrDefault();
                    var r = Result.GetValueOrDefault();
                    if (default(Maths).GreaterThan(i, r) || default(Maths).IsNaN(r))
                    {
                        Result = i;
                    }
                }
            }
        }

        public override ChainStatus ProcessNext(T? input)
        {
            var logic = new Logic(Result);
            logic.Process(input);
            Result = logic.Result;

            return ChainStatus.Flow;
        }

        void Optimizations.IHeadStart<T?>.Execute(ReadOnlySpan<T?> source)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                logic.Process(input);
            }

            Result = logic.Result;
        }

        void Optimizations.IHeadStart<T?>.Execute<Enumerator>(Optimizations.ITypedEnumerable<T?, Enumerator> source)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                logic.Process(input);
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.Select<S>(ReadOnlySpan<S> source, Func<S, T?> selector)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                logic.Process(selector(input));
            }

            Result = logic.Result;
        }

        ChainStatus Optimizations.ITailEnd<T?>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T?> resultSelector)
        {
            var logic = new Logic(Result);

            foreach (var input in span)
            {
                logic.Process(resultSelector(source, input));
            }

            Result = logic.Result;

            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T?>.Where(ReadOnlySpan<T?> source, Func<T?, bool> predicate)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    logic.Process(input);
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.Where<Enumerator>(Optimizations.ITypedEnumerable<T?, Enumerator> source, Func<T?, bool> predicate)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    logic.Process(input);
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    logic.Process(selector(input));
                }
            }

            Result = logic.Result;
        }

        void Optimizations.ITailEnd<T?>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T?> selector)
        {
            var logic = new Logic(Result);

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    logic.Process(selector(input));
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
