using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class Skip<T>
        : Link<T, T>
        , Optimizations.IMergeSkip<T>
        , Optimizations.ICountOnConsumableLink
    {
        private int _toSkip;

        public Skip(int toSkip) =>
            _toSkip = toSkip;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(_toSkip, activity);

        int Optimizations.ICountOnConsumableLink.GetCount(int count)
        {
            checked
            {
                return Math.Max(0, count - _toSkip);
            }
        }

        Consumable<T> Optimizations.IMergeSkip<T>.MergeSkip(ConsumableCons<T> consumable, int count)
        {
            if ((long)_toSkip + count > int.MaxValue)
                return consumable.AddTail(new Skip<T>(count));

            var totalCount = _toSkip + count;
            return consumable.ReplaceTailLink(new Skip<T>(totalCount));
        }

        sealed class Activity : Activity<T, T>
        {
            private readonly int _toSkip;

            private int _index;

            public Activity(int toSkip, Chain<T> next) : base(next) =>
                (_toSkip, _index) = (toSkip, 0);

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    _index++;
                }

                if (_index <= _toSkip)
                {
                    return ChainStatus.Filter;
                }
                return Next(input);
            }
        }
    }
}
