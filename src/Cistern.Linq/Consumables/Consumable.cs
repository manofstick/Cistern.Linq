using System.Diagnostics;

namespace Cistern.Linq.Consumables
{
    internal abstract class Consumable<T, U> : Consumable<U>
    {
        private readonly ILink<T, U> _linkOrNull;
        public ILink<T, U> Link => _linkOrNull ?? (ILink<T, U>)Links.Identity<T>.Instance;

        protected Consumable(ILink<T, U> link) =>
            _linkOrNull = link;

        public abstract IConsumable<U> Create(ILink<T, U> first);
        public override IConsumable<U> AddTail(ILink<U, U> next) => Create(_linkOrNull == null ? (ILink<T,U>)next : new Links.Composition<T, U, U>(_linkOrNull, next));

        public abstract IConsumable<V> Create<V>(ILink<T, V> first);
        public override IConsumable<V> AddTail<V>(ILink<U, V> next) => Create(_linkOrNull == null ? (ILink<T, V>)next : new Links.Composition<T, U, V>(_linkOrNull, next));

        protected bool IsIdentity => _linkOrNull == null;

        public override object TailLink => _linkOrNull is Links.Composition<T, U> c ? c.TailLink : _linkOrNull;

        public override IConsumable<V> ReplaceTailLink<Unknown,V>(ILink<Unknown,V> newLink)
        {
            if (_linkOrNull is Links.Composition<T, U> composition)
            {
                return Create(composition.ReplaceTail(newLink));
            }

            Debug.Assert(typeof(Unknown) == typeof(T));
            return Create((ILink<T,V>)(object)newLink);
        }
    }
}
