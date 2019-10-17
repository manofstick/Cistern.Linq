using System;

namespace Cistern.Linq.Links
{
    sealed class WhereSelect<T, U>
        : ILink<T, U>
        , Optimizations.IMergeSelect<U>
    {
        public Func<T, bool> Predicate { get; }
        public Func<T, U> Selector { get; }

        public WhereSelect(Func<T, bool> predicate, Func<T, U> selector) =>
            (Predicate, Selector) = (predicate, selector);

        Chain<T> ILink<T,U>.Compose(Chain<U> activity) =>
            new Activity(Predicate, Selector, activity);

        public IConsumable<V> MergeSelect<V>(Consumable<U> consumable, Func<U, V> u2v) =>
            consumable.ReplaceTailLink(new WhereSelect<T, V>(Predicate, t => u2v(Selector(t))));

        sealed class Activity
            : Activity<T, U>
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, bool> _predicate;
            private readonly Func<T, U> _selector; 

            public Activity(Func<T, bool> predicate, Func<T, U> selector, Chain<U> next) : base(next) =>
                (_predicate, _selector) = (predicate, selector);

            public override ChainStatus ProcessNext(T input) =>
                _predicate(input) ? Next(_selector(input)) : ChainStatus.Filter;

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                if (next is Optimizations.ITailEnd<U> optimized)
                {
                    return optimized.WhereSelect(source, _predicate, _selector);
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (_predicate(item))
                        {
                            var state = Next(_selector(item));
                            if (state.IsStopped())
                                return state;
                        }
                    }
                    return ChainStatus.Flow;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
            {
                if (next is Optimizations.ITailEnd<U> optimized)
                {
                    return optimized.WhereSelect<Enumerable, Enumerator, T>(source, _predicate, _selector);
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (_predicate(item))
                        {
                            var state = Next(_selector(item));
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
