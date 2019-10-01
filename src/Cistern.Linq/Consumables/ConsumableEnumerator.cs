using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    abstract class ConsumableEnumerator<V>
        : ConsumableCons<V>
        , IEnumerable<V>
        , IEnumerator<V>
    {
        private readonly int _threadId;
        internal int _state;
        internal V _current;

        protected ConsumableEnumerator() => (_state, _threadId) = (0, Environment.CurrentManagedThreadId);

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
            ConsumableEnumerator<V> enumerator = _state == 0 && _threadId == Environment.CurrentManagedThreadId ? this : Clone();
            enumerator._state = 1;
            return enumerator;
        }

        internal abstract ConsumableEnumerator<V> Clone();

        public abstract bool MoveNext();
    }
}
