using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class Concat_Deprecated<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        /// <summary>
        /// Used for Prepender in Prepend call
        /// </summary>
        private readonly IEnumerable<T> _firstOrNull;
        private readonly IEnumerable<T> _second;
        /// <summary>
        /// Used for Appender in Append call
        /// </summary>
        private readonly IEnumerable<T> _thirdOrNull;

        public Concat_Deprecated(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Link<T, V> link) : base(link) =>
            (_firstOrNull, _second, _thirdOrNull) = (firstOrNull, second, thirdOrNull);

        public override Consumable<V> Create   (Link<T, V> link) => new Concat_Deprecated<T, V>(_firstOrNull, _second, _thirdOrNull, link);
        public override Consumable<W> Create<W>(Link<T, W> link) => new Concat_Deprecated<T, W>(_firstOrNull, _second, _thirdOrNull, link);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Concat.Get(_firstOrNull, _second, _thirdOrNull, Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.Concat_Deprecated.Invoke(_firstOrNull, _second, _thirdOrNull, Link, consumer);

        public Consumable<V> Append(IEnumerable<V> next)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull == null)
                {
                    Debug.Assert(_firstOrNull != null);
                    return new Concat_Deprecated<T, V>(_firstOrNull, _second, (IEnumerable<T>)next, Link);
                }

                if (_firstOrNull == null)
                {
                    Debug.Assert(_thirdOrNull != null);
                    return new Concat_Deprecated<T, V>(_second, _thirdOrNull, (IEnumerable<T>)next, Link);
                }
            }

            return new Concat_Deprecated<V, V>(this, next, null, Links.Identity<V>.Instance);
        }

        public Consumable<V> Prepend(IEnumerable<V> prior)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull == null)
                {
                    Debug.Assert(_firstOrNull != null);
                    return new Concat_Deprecated<T, V>((IEnumerable<T>)prior, _firstOrNull, _second, Link);
                }

                if (_firstOrNull == null)
                {
                    Debug.Assert(_thirdOrNull != null);
                    return new Concat_Deprecated<T, V>((IEnumerable<T>)prior, _second, _thirdOrNull, Link);
                }
            }

            return new Concat_Deprecated<V, V>(null, prior, this, Links.Identity<V>.Instance);
        }

        public Consumable<V> Append(V element)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull is Appender<V> appender)
                {
                    return new Concat_Deprecated<T, V>(_firstOrNull, _second, (IEnumerable<T>)(object)appender.Add(element), Link);
                }
            }
            return Append(new Appender<V>(element));
        }

        public Consumable<V> Prepend(V element)
        {
            if (IsIdentity)
            {
                if (_firstOrNull is Prepender<V> prepender)
                {
                    return new Concat_Deprecated<T, V>((IEnumerable<T>)(object)prepender.Push(element), _second, _thirdOrNull, Link);
                }
            }
            return Prepend(new Prepender<V>(element));
        }
    }
}
