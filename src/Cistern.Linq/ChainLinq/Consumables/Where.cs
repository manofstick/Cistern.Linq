using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class WhereArray<T>
        : ConsumableEnumerator<T>
        , Optimizations.IMergeSelect<T>
        , Optimizations.IMergeWhere<T>
    {
        internal T[] Underlying { get; }
        internal Func<T, bool> Predicate { get; }

        int _idx;

        public WhereArray(T[] array, Func<T, bool> predicate) =>
            (Underlying, Predicate) = (array, predicate);

        public override void Consume(Consumer<T> consumer)
        {
            if (consumer is Optimizations.ITailEnd<T> optimized)
            {
                try
                {
                    optimized.Where(Underlying, Predicate);
                    consumer.ChainComplete();
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                ChainLinq.Consume.ReadOnlySpan.Invoke(Underlying, new Links.Where<T>(Predicate), consumer);
            }
        }

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

        public override object TailLink => this; // for IMergeSelect & IMergeWhere

        public override Consumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotImplementedException();

        public override Consumable<T> AddTail(ILink<T, T> transform) =>
            new Array<T, T>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override Consumable<U> AddTail<U>(ILink<T, U> transform) =>
            new Array<T, U>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        Consumable<V> Optimizations.IMergeSelect<T>.MergeSelect<V>(ConsumableCons<T> _, Func<T, V> u2v) =>
            new WhereSelectArray<T, V>(Underlying, Predicate, u2v);

        Consumable<T> Optimizations.IMergeWhere<T>.MergeWhere(ConsumableCons<T> _, Func<T, bool> predicate) =>
            new WhereArray<T>(Underlying, t => Predicate(t) && predicate(t));
    }

    sealed partial class WhereEnumerable<TEnumerable, TEnumerator, T>
        : ConsumableEnumerator<T>
        , Optimizations.IMergeSelect<T>
        , Optimizations.IMergeWhere<T>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerable, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }
        internal Func<T, bool> Predicate { get; }

        TEnumerator _enumerator;

        public WhereEnumerable(TEnumerable enumerable, Func<T, bool> predicate) =>
            (Underlying, Predicate) = (enumerable, predicate);

        public override void Consume(Consumer<T> consumer)
        {
            if (consumer is Optimizations.ITailEnd<T> optimized)
            {
                try
                {
                    if (Underlying.TryGetSourceAsSpan(out var span))
                        optimized.Where(span, Predicate);
                    else
                        optimized.Where<TEnumerable, TEnumerator>(Underlying, Predicate);

                    consumer.ChainComplete();
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                if (Underlying.TryGetSourceAsSpan(out var span))
                    ChainLinq.Consume.ReadOnlySpan.Invoke(span, new Links.Where<T>(Predicate), consumer);
                else
                    ChainLinq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, T>(Underlying, new Links.Where<T>(Predicate), consumer);
            }
        }

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

        public override object TailLink => this; // for IMergeSelect & IMergeWhere

        public override Consumable<V1> ReplaceTailLink<Unknown, V1>(ILink<Unknown, V1> newLink) => throw new NotImplementedException();

        public override Consumable<T> AddTail(ILink<T, T> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, T>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override Consumable<V> AddTail<V>(ILink<T, V> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        Consumable<V> Optimizations.IMergeSelect<T>.MergeSelect<V>(ConsumableCons<T> consumable, Func<T, V> u2v) =>
            new WhereSelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, Predicate, u2v);

        Consumable<T> Optimizations.IMergeWhere<T>.MergeWhere(ConsumableCons<T> consumable, Func<T, bool> predicate) =>
            new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, t => Predicate(t) && predicate(t));
    }
}
