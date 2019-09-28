using System.Diagnostics;

namespace Cistern.Linq.ChainLinq.Links
{
    abstract class Composition<T, U>
        : ILink<T, U>
    {
        protected Composition() { }

        public abstract object TailLink { get; }

        public abstract Chain<T> Compose(Chain<U> activity);
        public abstract ILink<T, V> ReplaceTail<Unknown, V>(ILink<Unknown, V> newLink);
    }

    sealed class Composition<T, U, V>
        : Composition<T, V>
        , Optimizations.ILinkFastCount
    {
        private readonly ILink<T, U> _first;
        private readonly ILink<U, V> _second;

        public Composition(ILink<T, U> first, ILink<U, V> second) =>
            (_first, _second) = (first, second);

        public override Chain<T> Compose(Chain<V> next) =>
            _first.Compose(_second.Compose(next));

        public override object TailLink => _second;


        public override ILink<T, W> ReplaceTail<Unknown, W>(ILink<Unknown, W> newLink)
        {
            Debug.Assert(typeof(Unknown) == typeof(U));

            return new Composition<T, U, W>(_first, (ILink<U,W>)(object)newLink);
        }
        bool Optimizations.ILinkFastCount.SupportedAsConsumer => (_first, _second) switch
        {
            (Optimizations.ILinkFastCount first, Optimizations.ILinkFastCount second) => first.SupportedAsConsumer && second.SupportedAsConsumer,
            _ => false
        };

        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count)
        {
            if (_first is Optimizations.ILinkFastCount first && _second is Optimizations.ILinkFastCount second)
            {
                var tryCount = first.FastCountAdjustment(count);
                if (tryCount.HasValue)
                    return second.FastCountAdjustment(tryCount.Value);
            }
            return null;
        }
    }

    static class Composition
    {
        public static ILink<T, V> Create<T, U, V>(ILink<T, U> first, ILink<U, V> second)
        {
            var identity = Identity<U>.Instance;

            if (ReferenceEquals(identity, first))
            {
                return (ILink<T, V>)(object)second;
            }

            if (ReferenceEquals(identity, second))
            {
                return (ILink<T, V>)(object)first;
            }

            return new Composition<T, U, V>(first, second);
        }
    }
}
