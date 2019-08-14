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

        Link<T, TResult> _factory;
        internal override Chain StartOfChain => _chain;

        public Enumerable(TEnumerable enumerable, Link<T, TResult> factory) =>
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

        const int Initialization = 0;
        const int ReadEnumerator = 1;
        const int Finished = 2;
        const int PostFinished = 3;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _chain ?? _factory.Compose(this);
                    _factory = null;
                    _enumerator = _enumerable.GetEnumerator();
                    _enumerable = default;
                    _state = ReadEnumerator;
                    goto case ReadEnumerator;

                case ReadEnumerator:
                    if (status.IsStopped() || !_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        _enumerator = default;
                        _state = Finished;
                        goto case Finished;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadEnumerator);
                    goto case ReadEnumerator;

                case Finished:
                    Result = default;
                    _chain.ChainComplete();
                    _state = PostFinished;
                    return false;

                default:
                    return false;
            }
        }
    }
}
