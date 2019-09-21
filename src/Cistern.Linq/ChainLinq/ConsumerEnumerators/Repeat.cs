using System.Diagnostics;

namespace Cistern.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Repeat<T, TResult> : ConsumerEnumerator<TResult>
    {
        private readonly T _element;
        private readonly int _end;
        private Chain<T> _chain = null;
        private bool _completed;

        int _current;

        internal override Chain StartOfChain => _chain;

        public Repeat(T element, int count, ILink<T, TResult> factory)
        {
            Debug.Assert(count > 0);

            _element = element;

            _current = 0;
            _end = count;

            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            base.ChainDispose();
            _chain = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (_current == _end || status.IsStopped())
            {
                if (!_completed && _chain.ChainComplete(status & ~ChainStatus.Flow).NotStoppedAndFlowing())
                {
                    _completed = true;
                    return true;
                }
                Result = default;
                return false;
            }

            ++_current;

            status = _chain.ProcessNext(_element);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
