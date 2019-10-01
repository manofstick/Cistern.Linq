using System;
using System.Collections.Generic;

namespace Cistern.Linq.ConsumerEnumerators
{
    sealed class SelectMany<Enumerable, T, V> : ConsumerEnumerator<V>
        where Enumerable : IEnumerable<T>
    {
        /* Implementation of:

                    var consumer = new Consumer.SetResult<V>();
                    var chain = composed.Compose(consumer);
                    try
                    {
                        foreach (var e in selectMany)
                        {
                            foreach (var item in e)
                            {
                                var state = chain.ProcessNext(item);
                                if (state.IsFlowing())
                                    yield return consumer.Result;
                                if (state.IsStopped())
                                    break;
                            }
                        }
                        chain.ChainComplete();
                    }
                    finally
                    {
                        chain.ChainDispose();
                    }
        */

        const int Start = 0;
        const int OuterEnumeratorMoveNext = 1;
        const int InnerEnumeratorMoveNextOnSelf = 2;
        const int InnerEnumeratorMoveNext = 3;
        const int CheckStopped = 4;
        const int Completing = 5;
        const int Finished = 6;
        const int PostFinished = 7;

        int _state;

        Consumable<Enumerable> _consumable;
        ILink<T, V> _link;
        Chain<T> _chain;
        IEnumerator<Enumerable> _outer;
        IEnumerator<T> _inner;
        ChainStatus _status;

        public SelectMany(Consumable<Enumerable> selectMany, ILink<T, V> link)
        {
            _consumable = selectMany;
            _link = link;

            _state = Start;
        }

        public override void Dispose()
        {
            _state = PostFinished;

            if (_outer != null)
            {
                _outer.Dispose();
                _outer = null;
            }

            if (_inner != null)
            {
                _inner.Dispose();
                _inner = null;
            }

            if (_chain != null)
            {
                _chain.ChainDispose();
                _chain = null;
            }

            Result = default;
        }

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Start:
                    _chain = _link.Compose(this);
                    _link = null;

                    _outer = _consumable.GetEnumerator();
                    _consumable = null;

                    _state = OuterEnumeratorMoveNext;
                    goto case OuterEnumeratorMoveNext;

                case OuterEnumeratorMoveNext:
                    if (_outer.MoveNext())
                    {
                        _inner = _outer.Current.GetEnumerator();

                        if (ReferenceEquals(_chain, this))
                        {
                            _state = InnerEnumeratorMoveNextOnSelf;
                            goto case InnerEnumeratorMoveNextOnSelf;
                        }
                        else
                        {
                            _state = InnerEnumeratorMoveNext;
                            goto case InnerEnumeratorMoveNext;
                        }
                    }

                    _state = Completing;
                    goto case Completing;

                case InnerEnumeratorMoveNextOnSelf:
                    if (!_inner.MoveNext())
                    {
                        _inner.Dispose();
                        _inner = default;
                        _state = OuterEnumeratorMoveNext;
                        goto case OuterEnumeratorMoveNext;
                    }

                    Result = (V)(object)_inner.Current; // should be no-op as TResult should equal T
                    return true;

                case InnerEnumeratorMoveNext:
                    if (_inner.MoveNext())
                    {
                        _status = _chain.ProcessNext(_inner.Current);
                        if (_status.IsFlowing())
                        {
                            _state = CheckStopped;
                            return true;
                        }

                        _state = CheckStopped;
                        goto case CheckStopped;
                    }

                    _inner.Dispose();
                    _inner = null;

                    _state = OuterEnumeratorMoveNext;
                    goto case OuterEnumeratorMoveNext;

                case CheckStopped:
                    if (_status.IsStopped())
                    {
                        _inner.Dispose();
                        _inner = null;

                        _state = Completing;
                        goto case Completing;
                    }

                    _state = InnerEnumeratorMoveNext;
                    goto case InnerEnumeratorMoveNext;

                case Completing:
                    if (_chain.ChainComplete(status & ~ChainStatus.Flow).NotStoppedAndFlowing())
                    {
                        _state = Finished;
                        return true;
                    }
                    goto case Finished;

                case Finished:
                    Result = default;

                    _outer.Dispose();
                    _outer = null;

                    _chain.ChainDispose();
                    _chain = null;

                    _state = PostFinished;
                    goto default;

                default:
                    return false;
            }
        }
    }

    sealed class SelectMany<TSource, TCollection, T, V> : ConsumerEnumerator<V>
    {
        /* Implementation of:

                    var consumer = new Consumer.SetResult<V>();
                    var chain = link.Compose(consumer);
                    try
                    {
                        foreach (var (source, items) in selectMany)
                        {
                            foreach (var item in items)
                            {
                                var state = chain.ProcessNext(resultSelector(source, item));
                                if (state.IsFlowing())
                                    yield return consumer.Result;
                                if (state.IsStopped())
                                    break;
                            }
                        }
                        chain.ChainComplete();
                    }
                    finally
                    {
                        chain.ChainDispose();
                    }
        */

        const int Start = 0;
        const int OuterEnumeratorMoveNext = 1;
        const int InnerEnumeratorMoveNext = 2;
        const int CheckStopped = 3;
        const int Completing = 4;
        const int Finished = 5;
        const int PostFinished = 6;

        int _state;

        Consumable<(TSource, IEnumerable<TCollection>)> _consumable;
        ILink<T, V> _link;
        Func<TSource, TCollection, T> _resultSelector;
        Chain<T> _chain;
        IEnumerator<(TSource, IEnumerable<TCollection>)> _outer;
        TSource _source;
        IEnumerator<TCollection> _inner;
        ChainStatus _status;

        public SelectMany(Consumable<(TSource, IEnumerable<TCollection>)> selectMany, Func<TSource, TCollection, T> resultSelector, ILink<T, V> link)
        {
            _consumable = selectMany;
            _link = link;
            _resultSelector = resultSelector;

            _state = Start;
        }

        public override void Dispose()
        {
            _state = PostFinished;

            if (_outer != null)
            {
                _outer.Dispose();
                _outer = null;
            }

            if (_inner != null)
            {
                _inner.Dispose();
                _inner = null;
            }

            if (_chain != null)
            {
                _chain.ChainDispose();
                _chain = null;
            }

            _resultSelector = null;
            _source = default;
            Result = default;
        }

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Start:
                    _chain = _link.Compose(this);
                    _link = null;

                    _outer = _consumable.GetEnumerator();
                    _consumable = null;

                    _state = OuterEnumeratorMoveNext;
                    goto case OuterEnumeratorMoveNext;

                case OuterEnumeratorMoveNext:
                    if (_outer.MoveNext())
                    {
                        var (source, e) = _outer.Current;
                        _source = source;
                        _inner = e.GetEnumerator();

                        _state = InnerEnumeratorMoveNext;
                        goto case InnerEnumeratorMoveNext;
                    }

                    _state = Completing;
                    goto case Completing;

                case InnerEnumeratorMoveNext:
                    if (_inner.MoveNext())
                    {
                        _status = _chain.ProcessNext(_resultSelector(_source, _inner.Current));
                        if (_status.IsFlowing())
                        {
                            _state = CheckStopped;
                            return true;
                        }

                        _state = CheckStopped;
                        goto case CheckStopped;
                    }

                    _inner.Dispose();
                    _inner = null;

                    _state = OuterEnumeratorMoveNext;
                    goto case OuterEnumeratorMoveNext;

                case CheckStopped:
                    if (_status.IsStopped())
                    {
                        _inner.Dispose();
                        _inner = null;

                        _state = Completing;
                        goto case Completing;
                    }

                    _state = InnerEnumeratorMoveNext;
                    goto case InnerEnumeratorMoveNext;

                case Completing:
                    if (_chain.ChainComplete(status & ~ChainStatus.Flow).NotStoppedAndFlowing())
                    {
                        _state = Finished;
                        return true;
                    }
                    goto case Finished;

                case Finished:
                    _source = default;
                    Result = default;

                    _outer.Dispose();
                    _outer = null;

                    _chain.ChainDispose();
                    _chain = null;

                    _resultSelector = null;

                    _state = PostFinished;
                    goto default;

                default:
                    return false;
            }
        }
    }
}
