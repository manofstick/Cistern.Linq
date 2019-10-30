using System.Diagnostics;

namespace Cistern.Linq.Consumables
{
    internal abstract class Consumable<T, U> : Consumable<U>
    {
        public ILink<T, U> Link => LinkOrNull ?? (ILink<T, U>)Links.Identity<T>.Instance;

        public ILink<T, U> LinkOrNull { get; }

        protected Consumable(ILink<T, U> link) =>
            LinkOrNull = link;

        public abstract IConsumable<U> Create(ILink<T, U> first);
        public override IConsumable<U> AddTail(ILink<U, U> next) => Create(LinkOrNull == null ? (ILink<T,U>)next : new Links.Composition<T, U, U>(LinkOrNull, next));

        public abstract IConsumable<V> Create<V>(ILink<T, V> first);
        public override IConsumable<V> AddTail<V>(ILink<U, V> next) => Create(LinkOrNull == null ? (ILink<T, V>)next : new Links.Composition<T, U, V>(LinkOrNull, next));

        protected bool IsIdentity => LinkOrNull == null;

        public override object TailLink => LinkOrNull is Links.Composition<T, U> c ? c.TailLink : LinkOrNull;

        public override IConsumable<V> ReplaceTailLink<Unknown,V>(ILink<Unknown,V> newLink)
        {
            if (LinkOrNull is Links.Composition<T, U> composition)
            {
                return Create(composition.ReplaceTail(newLink));
            }

            Debug.Assert(typeof(Unknown) == typeof(T));
            return Create((ILink<T,V>)(object)newLink);
        }
    }
}
