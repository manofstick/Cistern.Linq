using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class SelectIndexedWhere<T, U>
        : ILink<T, U>
        , Optimizations.IMergeWhere<U>
    {
        public Func<T, int, U> Selector { get; }
        public Func<U, bool> Predicate { get; }

        public SelectIndexedWhere(Func<T, int, U> selector, Func<U, bool> predicate) =>
            (Selector, Predicate) = (selector, predicate);

        Chain<T> ILink<T,U>.Compose(Chain<U> activity) =>
            new Activity(Selector, Predicate, 0, activity);

        Consumable<U> Optimizations.IMergeWhere<U>.MergeWhere(ConsumableCons<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectIndexedWhere<T, U>(Selector, t => Predicate(t) && second(t)));

        sealed class Activity
            : Activity<T, U>
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, int, U> _selector;
            private readonly Func<U, bool> _predicate;

            private int _index;

            public Activity(Func<T, int, U> selector, Func<U, bool> predicate, int startIndex, Chain<U> next) : base(next)
            {
                (_selector, _predicate) = (selector, predicate);
                checked
                {
                    _index = startIndex - 1;
                }
            }

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    var item = _selector(input, ++_index);
                    return _predicate(item) ? Next(item) : ChainStatus.Filter;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> memory)
            {
                checked
                {
                    foreach (var t in memory)
                    {
                        var u = _selector(t, ++_index);
                        if (_predicate(u))
                        {
                            var state = Next(u);
                            if (state.IsStopped())
                                return state;
                        }
                    }
                    return ChainStatus.Flow;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
            {
                checked
                {
                    foreach (var t in source)
                    {
                        var u = _selector(t, ++_index);
                        if (_predicate(u))
                        {
                            var state = Next(u);
                            if (state.IsStopped())
                                return state;
                        }
                    }
                    return ChainStatus.Flow;
                }
            }
        }
    }
}
