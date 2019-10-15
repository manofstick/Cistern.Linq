using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
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
                    var status = optimized.Where(Underlying, Predicate);

                    consumer.ChainComplete(status & ~ChainStatus.Flow);
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                Cistern.Linq.Consume.ReadOnlySpan.Invoke(Underlying, new Links.Where<T>(Predicate), consumer);
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

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotImplementedException();

        public override IConsumable<T> AddTail(ILink<T, T> transform) =>
            new Array<T, T>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override IConsumable<U> AddTail<U>(ILink<T, U> transform) =>
            new Array<T, U>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        IConsumable<V> Optimizations.IMergeSelect<T>.MergeSelect<V>(IConsumable<T> _, Func<T, V> u2v) =>
            new WhereSelectArray<T, V>(Underlying, Predicate, u2v);

        IConsumable<T> Optimizations.IMergeWhere<T>.MergeWhere(IConsumable<T> _, Func<T, bool> predicate) =>
            new WhereArray<T>(Underlying, t => Predicate(t) && predicate(t));
    }

    partial class WhereEnumerable<TEnumerable, TEnumerator, T>
        : ConsumableEnumerator<T>
        , Optimizations.IMergeSelect<T>
        , Optimizations.IMergeWhere<T>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
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
                    var status = ChainStatus.Flow;
                    if (Underlying.TryGetSourceAsSpan(out var span))
                        status = optimized.Where(span, Predicate);
                    else
                        status = optimized.Where<TEnumerable, TEnumerator>(Underlying, Predicate);

                    consumer.ChainComplete(status & ~ChainStatus.Flow);
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                if (Underlying.TryGetSourceAsSpan(out var span))
                    Cistern.Linq.Consume.ReadOnlySpan.Invoke(span, new Links.Where<T>(Predicate), consumer);
                else
                    Cistern.Linq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, T>(Underlying, new Links.Where<T>(Predicate), consumer);
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

        public override IConsumable<V1> ReplaceTailLink<Unknown, V1>(ILink<Unknown, V1> newLink) => throw new NotImplementedException();

        public override IConsumable<T> AddTail(ILink<T, T> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, T>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public override IConsumable<V> AddTail<V>(ILink<T, V> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, Links.Composition.Create(new Links.Where<T>(Predicate), transform));

        public virtual IConsumable<V> MergeSelect<V>(IConsumable<T> consumable, Func<T, V> u2v) =>
            new WhereSelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, Predicate, u2v);

        public virtual IConsumable<T> MergeWhere(IConsumable<T> consumable, Func<T, bool> predicate) =>
            new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, t => Predicate(t) && predicate(t));
    }

    sealed class WhereList<T>
        : WhereEnumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T>
    {
        public WhereList(List<T> list, Func<T, bool> predicate)
            : base(new Optimizations.ListEnumerable<T>(list), predicate) {}

        public override IConsumable<V> MergeSelect<V>(IConsumable<T> consumable, Func<T, V> u2v) =>
            new WhereSelectList<T, V>(Underlying.List, Predicate, u2v);

        public override IConsumable<T> MergeWhere(IConsumable<T> consumable, Func<T, bool> predicate) =>
            new WhereList<T>(Underlying.List, t => Predicate(t) && predicate(t));

        public override void Consume(Consumer<T> consumer)
        {
            if (consumer is Optimizations.IPipelineList<T> pipeline)
            {
                try
                {
                    var status = pipeline.Where(Underlying.List, Predicate);
                    consumer.ChainComplete(status & ~ChainStatus.Flow);
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                base.Consume(consumer);
            }
        }

    }

}
