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
        , Optimizations.ICountOnConsumableLink
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

        int Optimizations.ICountOnConsumableLink.GetCount(int count)
        {
            if (_first is Optimizations.ICountOnConsumableLink first && _second is Optimizations.ICountOnConsumableLink second)
            {
                count = first.GetCount(count);
                return count < 0 ? count : second.GetCount(count);
            }
            return -1;
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
