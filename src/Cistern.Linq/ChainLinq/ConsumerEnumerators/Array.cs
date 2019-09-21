namespace Cistern.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Array<T, TResult> : ConsumerEnumerator<TResult>
    {
        private T[] _array;
        private readonly int _endIdx;
        private int _idx;
        private bool _completed;
        private Chain<T> _chain = null;

        internal override Chain StartOfChain => _chain;

        public Array(T[] array, int start, int length, ILink<T, TResult> factory)
        {
            _completed = false;
            _idx = start;
            checked { _endIdx = start + length; }

            _array = array;
            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            _array = null;
            _chain = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (_idx >= _endIdx || status.IsStopped())
            {
                if (!_completed && _chain.ChainComplete(status & ~ChainStatus.Flow).NotStoppedAndFlowing())
                {
                    _completed = true;
                    return true;
                }

                Result = default;
                return false;
            }

            status = _chain.ProcessNext(_array[_idx++]);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
