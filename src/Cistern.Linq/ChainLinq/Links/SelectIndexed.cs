using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed partial class SelectIndexed<T, U>
        : Link<T, U>
        , Optimizations.IMergeWhere<U>
    {
        readonly int _startIndex;
        readonly Func<T, int, U> _selector;

        private SelectIndexed(Func<T, int, U> selector, int startIndex) : base(LinkType.SelectIndexed) =>
            (_selector, _startIndex) = (selector, startIndex);

        public SelectIndexed(Func<T, int, U> selector) : this(selector, 0) { }
        public Consumable<U> MergeWhere(ConsumableCons<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectIndexedWhere<T, U>(_selector, second));

        public override Chain<T> Compose(Chain<U> activity) =>
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

            public void Execute(ReadOnlySpan<T> source)
            {
                checked
                {
                    foreach (var input in source)
                    {
                        var status = Next(_selector(input, ++_index));
                        if (status.IsStopped())
                            break;
                    }
                }
            }

            public void Execute<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source) where Enumerator : IEnumerator<T>
            {
                checked
                {
                    foreach (var input in source)
                    {
                        var status = Next(_selector(input, ++_index));
                        if (status.IsStopped())
                            break;
                    }
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
