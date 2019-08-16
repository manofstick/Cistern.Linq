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

        sealed class SelectManyOuterConsumer<T>
            : Consumer<IEnumerable<T>, ChainEnd>
            , Optimizations.ITailEnd<IEnumerable<T>>
        {
            private readonly Chain<T> _chainT;
            private UnknownEnumerable.ChainConsumer<T> _inner;

            public SelectManyOuterConsumer(Chain<T> chainT) : base(default) =>
                _chainT = chainT;

            public override ChainStatus ProcessNext(IEnumerable<T> input) =>
                UnknownEnumerable.Consume(input, _chainT, ref _inner);

            void Optimizations.ITailEnd<IEnumerable<T>>.Select<S>(ReadOnlySpan<S> source, Func<S, IEnumerable<T>> selector)
            {
                foreach (var s in source)
                {
                    var status = UnknownEnumerable.Consume(selector(s), _chainT, ref _inner);
                    if (status.IsStopped())
                        break;
                }
            }

            // Only Select and SelectIndexed are use for the outer part of SelectMany to collect the IEnumerable
            ChainStatus Optimizations.ITailEnd<IEnumerable<T>>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, IEnumerable<T>> resultSelector) => throw new NotSupportedException();
            void Optimizations.ITailEnd<IEnumerable<T>>.Where(ReadOnlySpan<IEnumerable<T>> source, Func<IEnumerable<T>, bool> predicate) => throw new NotSupportedException();
            public void Where<Enumerator>(Optimizations.ITypedEnumerable<IEnumerable<T>, Enumerator> source, Func<IEnumerable<T>, bool> predicate) where Enumerator : IEnumerator<IEnumerable<T>> => throw new NotSupportedException();
            void Optimizations.ITailEnd<IEnumerable<T>>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, IEnumerable<T>> selector) => throw new NotSupportedException();
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

        public static void Invoke<T, V>(Consumable<IEnumerable<T>> e, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                e.Consume(new SelectManyOuterConsumer<T>(chain));
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> e, Func<TSource, TCollection, T> resultSelector, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
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
    }
}
