using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class WhereArray<T> : ConsumableEnumerator<T>
    {
        internal T[] Underlying { get; }
        internal Func<T, bool> Predicate { get; }

        int _idx;

        public WhereArray(T[] array, Func<T, bool> predicate) =>
            (Underlying, Predicate) = (array, predicate);

        public override void Consume(Consumer<T> consumer) =>
            ChainLinq.Consume.ReadOnlyMemory.Invoke(Underlying, new Links.Where<T>(Predicate), consumer);

        internal override ConsumableEnumerator<T> Clone() =>
            new WhereArray<T>(Underlying, Predicate);

        public override bool MoveNext()
        {
            if (_state == 1)
            {
                while (_idx < Underlying.Length)
                {
                    var item = Underlying[_idx++];
                    if (Predicate(item))
                    {
                        _current = item;
                        return true;
                    }
                }
                _state = int.MaxValue;
            }

            _current = default(T);
            return false;
        }

        public override object TailLink => this;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink)
        {
            throw new NotImplementedException();
        }

        public override Consumable<T> AddTail(Link<T, T> transform) =>
            new Array<T, T>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override Consumable<U> AddTail<U>(Link<T, U> transform) =>
            new Array<T, U>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));
    }

    sealed partial class WhereEnumerable<TEnumerable, TEnumerator, T> : ConsumableEnumerator<T>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }
        internal Func<T, bool> Predicate { get; }

        TEnumerator _enumerator;

        public WhereEnumerable(TEnumerable enumerable, Func<T, bool> predicate) =>
            (Underlying, Predicate) = (enumerable, predicate);

        public override void Consume(Consumer<T> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, T>(Underlying, new Links.Where<T>(Predicate), consumer);

        internal override ConsumableEnumerator<T> Clone() =>
            new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, Predicate);

        public override void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = default;
            }
            base.Dispose();
        }

        public override bool MoveNext()
        {
            switch (_state)
            {
                case 1:
                    _enumerator = Underlying.GetEnumerator();
                    _state = 2;
                    goto case 2;

                case 2:
                    while (_enumerator.MoveNext())
                    {
                        var item = _enumerator.Current;
                        if (Predicate(item))
                        {
                            _current = item;
                            return true;
                        }
                    }
                    _state = int.MaxValue;
                    goto default;

                default:
                    _current = default(T);
                    if (_enumerator != null)
                    {
                        _enumerator.Dispose();
                        _enumerator = default;
                    }
                    return false;
            }
        }

        public override object TailLink => this;

        public override Consumable<V1> ReplaceTailLink<Unknown, V1>(Link<Unknown, V1> newLink) =>
            throw new NotImplementedException();

        public override Consumable<T> AddTail(Link<T, T> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, T>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override Consumable<V> AddTail<V>(Link<T, V> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));
    }
}
