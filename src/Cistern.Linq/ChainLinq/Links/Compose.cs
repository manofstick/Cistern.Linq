using System.Diagnostics;

namespace Cistern.Linq.ChainLinq.Links
{
    abstract class Composition<T, U>
        : Link<T, U>
    {
        protected Composition() { }

        public abstract object TailLink { get; }
        public abstract Link<T, V> ReplaceTail<Unknown, V>(Link<Unknown, V> newLink);
    }

    sealed class Composition<T, U, V>
        : Composition<T, V>
        , Optimizations.ICountOnConsumableLink
    {
        private readonly Link<T, U> _first;
        private readonly Link<U, V> _second;

        public Composition(Link<T, U> first, Link<U, V> second) =>
            (_first, _second) = (first, second);

        public override Chain<T> Compose(Chain<V> next) =>
            _first.Compose(_second.Compose(next));

        public override object TailLink => _second;

        public override Link<T, W> ReplaceTail<Unknown, W>(Link<Unknown, W> newLink)
        {
            Debug.Assert(typeof(Unknown) == typeof(U));

            return new Composition<T, U, W>(_first, (Link<U,W>)(object)newLink);
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
        public static Link<T, V> Create<T, U, V>(Link<T, U> first, Link<U, V> second)
        {
            var identity = Identity<U>.Instance;

            if (ReferenceEquals(identity, first))
            {
                return (Link<T, V>)(object)second;
            }

            if (ReferenceEquals(identity, second))
            {
                return (Link<T, V>)(object)first;
            }

            return new Composition<T, U, V>(first, second);
        }
    }
}
