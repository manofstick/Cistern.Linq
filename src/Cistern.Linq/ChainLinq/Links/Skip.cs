using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class Skip<T>
        : ILink<T, T>
        , Optimizations.ILinkFastCount
    {
        private int _toSkip;

        public Skip(int toSkip) =>
            _toSkip = toSkip;

        bool Optimizations.ILinkFastCount.SupportedAsConsumer => true;

        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count)
        {
            checked
            {
                return Math.Max(0, count - _toSkip);
            }
        }

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(_toSkip, activity);

        sealed class Activity
            : Activity<T, T>
            , Optimizations.IHeadStart<T>
        {
            private readonly int _toSkip;

            private int _index;

            public Activity(int toSkip, Chain<T> next) : base(next) =>
                (_toSkip, _index) = (toSkip, 0);

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                if (_toSkip >= source.Length)
                    return ChainStatus.Flow;

                source = source.Slice(_toSkip);

                if (next is Optimizations.IHeadStart<T> optimizations)
                    return optimizations.Execute(source);
                else
                {
                    foreach (var item in source)
                    {
                        var status = Next(item);
                        if (status.IsStopped())
                            return status;
                    }
                    return ChainStatus.Flow;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
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
