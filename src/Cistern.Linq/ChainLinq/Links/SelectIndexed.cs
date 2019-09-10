using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class SelectIndexed<T, U>
        : ILink<T, U>
        , Optimizations.IMergeWhere<U>
        , Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>
    {
        readonly int _startIndex;
        readonly Func<T, int, U> _selector;

        private SelectIndexed(Func<T, int, U> selector, int startIndex) =>
            (_selector, _startIndex) = (selector, startIndex);

        public SelectIndexed(Func<T, int, U> selector) : this(selector, 0) { }

        Consumable<U> Optimizations.IMergeWhere<U>.MergeWhere(ConsumableCons<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectIndexedWhere<T, U>(_selector, second));

        ILink<T, U> Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>.Skip(int toSkip)
        {
            checked
            {
                return new SelectIndexed<T, U>(_selector, _startIndex + toSkip);
            }
        }

        Chain<T> ILink<T,U>.Compose(Chain<U> activity) =>
            new Activity(_selector, _startIndex, activity);

        sealed class Activity
            : Activity<T, U>
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, int, U> _selector;

            private int _index;

            public Activity(Func<T, int, U> selector, int startIndex, Chain<U> next) : base(next)
            {
                _selector = selector;
                checked
                {
                    _index = startIndex - 1;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                checked
                {
                    foreach (var input in source)
                    {
                        var status = Next(_selector(input, ++_index));
                        if (status.IsStopped())
                            return ChainStatus.Stop;
                    }
                    return ChainStatus.Flow;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
            {
                checked
                {
                    foreach (var input in source)
                    {
                        var status = Next(_selector(input, ++_index));
                        if (status.IsStopped())
                            return status;
                    }
                    return ChainStatus.Flow;
                }
            }

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    return Next(_selector(input, ++_index));
                }
            }
        }
    }
}
