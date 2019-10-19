using System;

namespace Cistern.Linq.Consumer
{
    static class AggregateImpl
    {
        public static ChainStatus Execute<T, TAccumulate>(ReadOnlySpan<T> source, ref Func<TAccumulate, T, TAccumulate> func, ref TAccumulate accumulate)
        {
            // playing around a bit here, I see that when I don't pass the Func by reference, the C# compiler just 
            // removes the copy (which I was just playing with to see if it created better performance).
            // Anyway, maybe investigate more at some later stage...
            var f = func;
            var a = accumulate;

            var i = 0;
            for (; i < source.Length - 4 + 1; i += 4)
            {
                var x = (source[i+0], source[i+1], source[i+2], source[i+3]);
                a = f(f(f(f(a,x.Item1),x.Item2),x.Item3),x.Item4);
            }

            for (; i < source.Length; ++i)
                a = f(a, source[i]);

            accumulate = a;

            return ChainStatus.Flow;
        }

        public static ChainStatus Execute<T, TAccumulate, Enumerable, Enumerator>(Enumerable source, ref Func<TAccumulate, T, TAccumulate> func, ref TAccumulate accumulate)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
                a = f(a, input);

            accumulate = a;

            return ChainStatus.Flow;
        }

        public static ChainStatus Select<T, TAccumulate, S>(ReadOnlySpan<S> source, ref Func<TAccumulate, T, TAccumulate> func, ref TAccumulate accumulate, ref Func<S, T> selector)
        {
            var f = func;
            var a = accumulate;
            var s = selector;

            var i = 0;
            for (; i < source.Length - 4 + 1; i += 4)
            {
                var x = (source[i + 0], source[i + 1], source[i + 2], source[i + 3]);
                a = f(f(f(f(a, s(x.Item1)), s(x.Item2)), s(x.Item3)), s(x.Item4));
            }

            for (; i < source.Length; ++i)
                a = f(a, s(source[i]));

            accumulate = a;

            return ChainStatus.Flow;
        }

        public static TAccumulate Select<S, T, TAccumulate, Enumerable, Enumerator>(Enumerable source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
                a = f(a, selector(input));

            return a;
        }

        public static TAccumulate Where<T, TAccumulate>(ReadOnlySpan<T> source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, Func<T, bool> predicate)
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    a = f(a, input);
            }

            return a;
        }

        public static TAccumulate Where<T, TAccumulate, Enumerable, Enumerator>(Enumerable source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, Func<T, bool> predicate)
            where Enumerable : Optimizations.ITypedEnumerable<T, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    a = f(a, input);
            }

            return a;
        }

        public static TAccumulate SelectMany<T, TAccumulate, TSource, TCollection>(TSource source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            var f = func;
            var a = accumulate;

            foreach (var input in span)
            {
                a = f(a, resultSelector(source, input));
            }

            return a;
        }

        public static TAccumulate WhereSelect<S, T, TAccumulate>(ReadOnlySpan<S> source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, Func<S, bool> predicate, Func<S, T> selector)
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    a = f(a, selector(input));
            }

            return a;
        }

        public static TAccumulate WhereSelect<S, T, TAccumulate, Enumerable, Enumerator>(Enumerable source, Func<TAccumulate, T, TAccumulate> func, TAccumulate accumulate, Func<S, bool> predicate, Func<S, T> selector)
            where Enumerable : Optimizations.ITypedEnumerable<S, Enumerator>
            where Enumerator : System.Collections.Generic.IEnumerator<S>
        {
            var f = func;
            var a = accumulate;

            foreach (var input in source)
            {
                if (predicate(input))
                    a = f(a, selector(input));
            }

            return a;
        }
    }

    sealed class Aggregate<T, TAccumulate, TResult>
        : Consumer<T, TResult>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
        , Cache.IClean
    {
        /*readonly*/ Func<TAccumulate, T, TAccumulate> _func;
        /*readonly*/ Func<TAccumulate, TResult> _resultSelector;
        
        TAccumulate _accumulate;

        private Aggregate() : base(default) { }
            
        private void Init(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) =>
            (_accumulate, _func, _resultSelector) = (seed, func, resultSelector);

        public static Aggregate<T, TAccumulate, TResult> FactoryCreate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            var cached = Cache.TryGet<Aggregate<T, TAccumulate, TResult>>() ?? new Aggregate<T, TAccumulate, TResult>();
            cached.Init(seed, func, resultSelector);
            return cached;
        }

        void IDisposable.Dispose() => Cache.Stash(this);

        void Cache.IClean.Clean() => Init(default, default, default);

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

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source) =>
            AggregateImpl.Execute(source, ref _func, ref _accumulate);

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source) =>
            AggregateImpl.Execute<T, TAccumulate, Enumerable, Enumerator>(source, ref _func, ref _accumulate);

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector) =>
            AggregateImpl.Select(source, ref _func, ref _accumulate, ref selector);

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            _accumulate = AggregateImpl.Select<S, T, TAccumulate, Enumerable, Enumerator>(source, _func, _accumulate, selector);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            _accumulate = AggregateImpl.Where(source, _func, _accumulate, predicate);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            _accumulate = AggregateImpl.Where<T, TAccumulate, Enumerable, Enumerator>(source, _func, _accumulate, predicate);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            _accumulate = AggregateImpl.SelectMany(source, _func, _accumulate, span, resultSelector);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            _accumulate = AggregateImpl.WhereSelect(source, _func, _accumulate, predicate, selector);
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            _accumulate = AggregateImpl.WhereSelect<S, T, TAccumulate, Enumerable, Enumerator>(source, _func, _accumulate, predicate, selector);
            return ChainStatus.Flow;
        }
    }
}
