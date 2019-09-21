using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Enumerable<TEnumerable, TEnumerator, T, TResult> : ConsumerEnumerator<TResult>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        private TEnumerable _enumerable;
        private TEnumerator _enumerator;
        private Chain<T> _chain = null;
        int _state;

        ILink<T, TResult> _factory;
        internal override Chain StartOfChain => _chain;

        public Enumerable(TEnumerable enumerable, ILink<T, TResult> factory) =>
            (_enumerable, _factory, _state) = (enumerable, factory, Initialization);

        public override void ChainDispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = default;
            }
            _enumerable = default;
            _factory = null;
            _chain = null;
        }

        const int ReadEnumerator = 0;
        const int ReadEnumeratorOnSelf = 1; // optimization for when chain is Identity
        const int Completing = 2;
        const int Finished = 3;
        const int PostFinished = 4;
        const int Initialization = 5;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization: return MoveNext_Initialization();

                case ReadEnumerator:
                    if (status.IsStopped() || !_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        _enumerator = default;
                        _state = Completing;
                        goto case Completing;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadEnumerator);
                    goto case ReadEnumerator;

                case ReadEnumeratorOnSelf:
                    if (!_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        _enumerator = default;
                        _state = Completing;
                        goto case Completing;
                    }

                    Result = (TResult)(object)_enumerator.Current; // should be no-op as TResult should equal T
                    return true;

                case Completing: return MoveNext_Completing();

                case Finished: return MoveNext_Finished();

                default: return false;
            }
        }

        private bool MoveNext_Completing()
        {
            if (_chain.ChainComplete(status & ~ChainStatus.Flow).NotStoppedAndFlowing())
            {
                _state = Finished;
                return true;
            }
            return MoveNext_Finished();
        }

        private bool MoveNext_Finished()
        {
            Result = default;
            _state = PostFinished;
            return false;
        }

        private bool MoveNext_Initialization()
        {
            _chain = _chain ?? _factory.Compose(this);
            _factory = null;
            _enumerator = _enumerable.GetEnumerator();
            _enumerable = default;
            _state = ReferenceEquals(this, _chain) ? ReadEnumeratorOnSelf : ReadEnumerator;
            return MoveNext();
        }
    }
}
