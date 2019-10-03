using System;
using System.Collections.Generic;
using System.Threading;

namespace Cistern.Linq.Consumables
{
    abstract class ConsumableEnumerator<V>
        : Consumable<V>
        , IEnumerator<V>
    {
        internal int _state;
        internal V _current;

        protected ConsumableEnumerator() => _state = 0;

        V IEnumerator<V>.Current => _current;
        object System.Collections.IEnumerator.Current => _current;

        void System.Collections.IEnumerator.Reset() => ThrowHelper.ThrowNotSupportedException();

        public virtual void Dispose()
        {
            _state = int.MaxValue;
            _current = default(V);
        }

        public override IEnumerator<V> GetEnumerator()
        {
            ConsumableEnumerator<V> enumerator = Interlocked.CompareExchange(ref _state, 1, 0) == 0 ? this : Clone();
            enumerator._state = 1;
            return enumerator;
        }

        internal abstract ConsumableEnumerator<V> Clone();

        public abstract bool MoveNext();
    }
}
