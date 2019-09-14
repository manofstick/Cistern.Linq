using System;
using System.Collections.Generic;
using Cistern.Linq.ChainLinq.Optimizations;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class Skip<T>
        : ILink<T, T>
        , Optimizations.IMergeSkip<T>
        , Optimizations.ICountOnConsumableLink
    {
        private int _toSkip;

        public Skip(int toSkip) =>
            _toSkip = toSkip;

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
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

        sealed class Activity
            : Activity<T, T>
            , Optimizations.IHeadStart<T>
        {
            private readonly int _toSkip;

            private int _index;

            public Activity(int toSkip, Chain<T> next) : base(next) =>
                (_toSkip, _index) = (toSkip, 0);

            public ChainStatus Execute(ReadOnlySpan<T> source)
            {
                for (var i = _toSkip; i < source.Length; ++i)
                {
                    var status = Next(source[i]);
                    if (status.IsStopped())
                        return status;
                }

                return ChainStatus.Flow;
            }

            public ChainStatus Execute<Enumerable, Enumerator>(Enumerable source)
                where Enumerable : ITypedEnumerable<T, Enumerator>
                where Enumerator : IEnumerator<T>
            {
                using (var e = source.GetEnumerator())
                {
                    bool moveNext = true;
                    while (moveNext && _index < _toSkip)
                    {
                        _index++;
                        moveNext = e.MoveNext();
                    }

                    while (e.MoveNext())
                    {
                        var status = Next(e.Current);
                        if (status.IsStopped())
                            return status;
                    }
                }

                return ChainStatus.Flow;
            }

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
