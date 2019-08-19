using System;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class First<T> : Consumer<T, T>
    {
        private bool _found;
        private bool _orDefault;

        public First(bool orDefault) : base(default) =>
            (_orDefault, _found) = (orDefault, false);

        public override ChainStatus ProcessNext(T input)
        {
            _found = true;
            Result = input;
            return ChainStatus.Stop;
        }

        public override void ChainComplete()
        {
            if (!_orDefault && !_found)
            {
                ThrowHelper.ThrowNoElementsException();
            }
        }
    }
}
