using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class WhereSelect<T, U>
        : Link<T, U>
        , Optimizations.IMergeSelect<U>
    {
        public Func<T, bool> Predicate { get; }
        public Func<T, U> Selector { get; }

        public WhereSelect(Func<T, bool> predicate, Func<T, U> selector) : base(LinkType.WhereSelect) =>
            (Predicate, Selector) = (predicate, selector);

        public override Chain<T> Compose(Chain<U> activity) =>
            new Activity(Predicate, Selector, activity);

        public Consumable<V> MergeSelect<V>(ConsumableForMerging<U> consumable, Func<U, V> u2v) =>
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
            
            void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> span)
            {
                if (next is Optimizations.ITailWhereSelect<U> optimized)
                {
                    optimized.WhereSelect(span, _predicate, _selector);
                }
                else
                {
                    foreach (var item in span)
                    {
                        if (_predicate(item))
                        {
                            var state = Next(_selector(item));
                            if (state.IsStopped())
                                break;
                        }
                    }
                }
            }

            void Optimizations.IHeadStart<T>.Execute(List<T> list)
            {
                foreach (var item in list)
                {
                    if (_predicate(item))
                    {
                        var state = Next(_selector(item));
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            void Optimizations.IHeadStart<T>.Execute(IList<T> list, int start, int count)
            {
                for(var i=start; i < start+count; ++i)
                {
                    var item = list[i];
                    if (_predicate(item))
                    {
                        var state = Next(_selector(item));
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            void Optimizations.IHeadStart<T>.Execute(IEnumerable<T> enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (_predicate(item))
                    {
                        var state = Next(_selector(item));
                        if (state.IsStopped())
                            break;
                    }
                }
            }
        }
    }
}
