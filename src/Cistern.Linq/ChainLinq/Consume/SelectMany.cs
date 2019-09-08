using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class SelectMany
    {
        sealed class SelectManyInnerConsumer<TSource, TCollection, T> : Consumer<TCollection, ChainStatus>
        {
            private readonly Chain<T> _chainT;
            private readonly Func<TSource, TCollection, T> _resultSelector;

            public TSource Source { get; set; }

            public SelectManyInnerConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(ChainStatus.Flow) =>
                (_chainT, _resultSelector) = (chainT, resultSelector);

            public override ChainStatus ProcessNext(TCollection input)
            {
                var state = _chainT.ProcessNext(_resultSelector(Source, input));
                Result = state;
                return state;
            }
        }

        sealed class SelectManyOuterConsumer<Enumerable, T>
            : Consumer<Enumerable, ChainEnd>
            , Optimizations.ITailEnd<IEnumerable<T>>
            , Optimizations.IHeadStart<IEnumerable<T>>
            where Enumerable : IEnumerable<T>
        {
            private readonly Chain<T> _chainT;
            private UnknownEnumerable.ChainConsumer<T> _inner;

            public SelectManyOuterConsumer(Chain<T> chainT) : base(default) =>
                _chainT = chainT;

            ChainStatus Optimizations.IHeadStart<IEnumerable<T>>.Execute(ReadOnlySpan<IEnumerable<T>> source)
            {
                foreach (var s in source)
                {
                    var status = UnknownEnumerable.Consume(s, _chainT, ref _inner);
                    if (status.IsStopped())
                        return status;
                }
                return ChainStatus.Flow;
            }

            public override ChainStatus ProcessNext(Enumerable input) =>
                UnknownEnumerable.Consume(input, _chainT, ref _inner);

            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.Select<S>(ReadOnlySpan<S> source, Func<S, IEnumerable<T>> selector)
            {
                foreach (var s in source)
                {
                    var status = UnknownEnumerable.Consume(selector(s), _chainT, ref _inner);
                    if (status.IsStopped())
                        return status;
                }
                return ChainStatus.Flow;
            }

            // Only Concat, Select and SelectIndexed are use for the outer part of SelectMany to collect the IEnumerable
            ChainStatus Optimizations.IHeadStart<IEnumerable<T>>.Execute<Enumerable1, Enumerator>(Enumerable1 source) => throw new NotSupportedException();
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, IEnumerable<T>> resultSelector) => throw new NotSupportedException();
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.Where(ReadOnlySpan<IEnumerable<T>> source, Func<IEnumerable<T>, bool> predicate) => throw new NotSupportedException();
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.Where<WEnumerable, Enumerator>(WEnumerable source, Func<IEnumerable<T>, bool> predicate) => throw new NotSupportedException();
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, IEnumerable<T>> selector) => throw new NotSupportedException();
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.WhereSelect<WEnumerable, Enumerator, S>(WEnumerable source, Func<S, bool> predicate, Func<S, IEnumerable<T>> selector) => throw new NotSupportedException();
        }

        sealed class SelectManyOuterConsumer<TSource, TCollection, T> : Consumer<(TSource, IEnumerable<TCollection>), ChainEnd>
        {
            readonly Func<TSource, TCollection, T> _resultSelector;
            readonly Chain<T> _chainT;

            SelectManyInnerConsumer<TSource, TCollection, T> _inner;

            private SelectManyInnerConsumer<TSource, TCollection, T> GetInnerConsumer()
            {
                if (_inner == null)
                    _inner = new SelectManyInnerConsumer<TSource, TCollection, T>(_resultSelector, _chainT);
                return _inner;
            }

            public SelectManyOuterConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(default(ChainEnd)) =>
                (_chainT, _resultSelector) = (chainT, resultSelector);

            public override ChainStatus ProcessNext((TSource, IEnumerable<TCollection>) input)
            {
                var state = ChainStatus.Flow;
                if (input.Item2 is Consumable<TCollection> consumable)
                {
                    var consumer = GetInnerConsumer();
                    consumer.Source = input.Item1;
                    consumable.Consume(consumer);
                    state = consumer.Result;
                }
                else if (input.Item2 is TCollection[] array)
                {
                    if (_chainT is Optimizations.ITailEnd<T> optimized)
                    {
                        state = optimized.SelectMany(input.Item1, array, _resultSelector);
                    }
                    else
                    {
                        foreach (var item in array)
                        {
                            state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                            if (state.IsStopped())
                                break;
                        }
                    }
                }
                else if (input.Item2 is List<TCollection> list)
                {
                    foreach (var item in list)
                    {
                        state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                        if (state.IsStopped())
                            break;
                    }
                }
                else
                {
                    foreach (var item in input.Item2)
                    {
                        state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                        if (state.IsStopped())
                            break;
                    }
                }
                return state;
            }
        }

        public static void Invoke<Enumerable, T>(Consumable<Enumerable> e, Chain<T> chain)
            where Enumerable : IEnumerable<T>
        {
            try
            {
                e.Consume(new SelectManyOuterConsumer<Enumerable, T>(chain));
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TSource, TCollection, T>(Consumable<(TSource, IEnumerable<TCollection>)> e, Func<TSource, TCollection, T> resultSelector, Chain<T> chain)
        {
            try
            {
                e.Consume(new SelectManyOuterConsumer<TSource, TCollection, T>(resultSelector, chain));
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<Enumerable, T, V>(Consumable<Enumerable> e, Link<T, V> composition, Chain<V> consumer)
            where Enumerable : IEnumerable<T> =>
            Invoke(e, composition.Compose(consumer));

        public static void Invoke<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> e, Func<TSource, TCollection, T> resultSelector, Link<T, V> composition, Chain<V> consumer) =>
            Invoke(e, resultSelector, composition.Compose(consumer));
    }
}
