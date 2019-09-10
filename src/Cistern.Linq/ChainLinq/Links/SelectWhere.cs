using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class SelectWhere<T, U>
        : Link<T, U>
        , Optimizations.IMergeWhere<U>
    {
        public Func<T, U> Selector { get; }
        public Func<U, bool> Predicate { get; }

        public SelectWhere(Func<T, U> selector, Func<U, bool> predicate) =>
            (Selector, Predicate) = (selector, predicate);

        public override Chain<T> Compose(Chain<U> activity) =>
            new Activity(Selector, Predicate, activity);

        Consumable<U> Optimizations.IMergeWhere<U>.MergeWhere(ConsumableCons<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectWhere<T, U>(Selector, t => Predicate(t) && second(t)));

        sealed class Activity
            : Activity<T, U>
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, U> _selector;
            private readonly Func<U, bool> _predicate;

            public Activity(Func<T, U> selector, Func<U, bool> predicate, Chain<U> next) : base(next) =>
                (_selector, _predicate) = (selector, predicate);

            public override ChainStatus ProcessNext(T input)
            {
                var item = _selector(input);
                return _predicate(item) ? Next(item) : ChainStatus.Filter;
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> memory)
            {
                foreach (var t in memory)
                {
                    var u = _selector(t);
                    if (_predicate(u))
                    {
                        var state = Next(u);
                        if (state.IsStopped())
                            return state;
                    }
                }
                return ChainStatus.Flow;
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
            {
                foreach (var t in source)
                {
                    var u = _selector(t);
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
