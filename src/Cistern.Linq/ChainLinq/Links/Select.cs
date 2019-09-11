using System;

namespace Cistern.Linq.ChainLinq.Links
{
    class Select<T, U>
        : ILink<T, U>
        , Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>
        , Optimizations.IMergeSelect<U>
        , Optimizations.IMergeWhere<U>
    {
        public Select(Func<T, U> selector) =>
            Selector = selector;

        public Func<T, U> Selector { get; }

        Chain<T> ILink<T,U>.Compose(Chain<U> activity) =>
            new Activity(Selector, activity);

        public virtual Consumable<V> MergeSelect<V>(ConsumableCons<U> consumable, Func<U, V> selector) =>
            consumable.ReplaceTailLink(new Select<T, U, V>(Selector, selector));

        Consumable<U> Optimizations.IMergeWhere<U>.MergeWhere(ConsumableCons<U> consumable, Func<U, bool> predicate) =>
            consumable.ReplaceTailLink(new SelectWhere<T, U>(Selector, predicate));

        ILink<T, U> Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>.Skip(int toSkip) => this;

        sealed partial class Activity
            : Activity<T, U>
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, U> _selector;

            public Activity(Func<T, U> selector, Chain<U> next) : base(next) =>
                _selector = selector;

            public override ChainStatus ProcessNext(T input) =>
                Next(_selector(input));

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                if (next is Optimizations.ITailEnd<U> optimized)
                {
                    return optimized.Select(source, _selector);
                }
                else
                {
                    foreach (var item in source)
                    {
                        var state = Next(_selector(item));
                        if (state.IsStopped())
                            return state;
                    }
                    return ChainStatus.Flow;
                }
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
            {
                if (next is Optimizations.ITailEnd<U> optimized)
                {
                    return optimized.Select<Enumerable, Enumerator, T>(source, _selector);
                }
                else
                {
                    foreach (var item in source)
                    {
                        var state = Next(_selector(item));
                        if (state.IsStopped())
                            return state;
                    }
                    return ChainStatus.Flow;
                }
            }
        }
    }

    sealed class Select<T, U, V> : Select<T, V>
    {
        private readonly Func<T, U> _t2u;
        private readonly Func<U, V> _u2v;

        public Select(Func<T, U> t2u, Func<U, V> u2v) : base(t => u2v(t2u(t))) =>
            (_t2u, _u2v) = (t2u, u2v);

        public override Consumable<W> MergeSelect<W>(ConsumableCons<V> consumer, Func<V, W> v2w) =>
            consumer.ReplaceTailLink(new Select<T, U, V, W>(_t2u, _u2v, v2w));
    }

    sealed class Select<T, U, V, W> : Select<T, W>
    {
        private readonly Func<T, U> _t2u;
        private readonly Func<U, V> _u2v;
        private readonly Func<V, W> _v2w;

        public Select(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w) : base(t => v2w(u2v(t2u(t)))) =>
            (_t2u, _u2v, _v2w) = (t2u, u2v, v2w);

        public override Consumable<X> MergeSelect<X>(ConsumableCons<W> consumer, Func<W, X> w2x) =>
            consumer.ReplaceTailLink(new Select<T, U, V, W, X>(_t2u, _u2v, _v2w, w2x));
    }

    sealed class Select<T, U, V, W, X> : Select<T, X>
    {
        public Select(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W, X> w2x)
            : base(t => w2x(v2w(u2v(t2u(t)))))
        { }
    }
}
