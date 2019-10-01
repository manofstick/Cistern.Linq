using System.Diagnostics;

namespace Cistern.Linq.ConsumerEnumerators
{
    internal sealed class Range<TResult> : ConsumerEnumerator<TResult>
    {
        private readonly int _end;
        private Chain<int> _chain = null;
        bool _completed;
        int _current;

        internal override Chain StartOfChain => _chain;

        public Range(int start, int count, ILink<int, TResult> factory)
        {
            Debug.Assert(count > 0);

            _completed = false;
            _current = start;
            _end = unchecked(start + count);

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

            status = _chain.ProcessNext(_current++);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
