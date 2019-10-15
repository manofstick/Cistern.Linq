using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed partial class WhereSelectArray<T, U>
        : ConsumableEnumerator<U>
        , Optimizations.IMergeSelect<U>
    {
        internal T[] Underlying { get; }
        internal Func<T, bool> Predicate { get; }
        internal Func<T, U> Selector { get; }

        int _idx;

        public WhereSelectArray(T[] array, Func<T, bool> predicate, Func<T, U> selector) =>
            (Underlying, Predicate, Selector) = (array, predicate, selector);

        public override void Consume(Consumer<U> consumer)
        {
            if (consumer is Optimizations.ITailEnd<U> optimized)
            {
                try
                {
                    var status = optimized.WhereSelect(Underlying, Predicate, Selector);

                    consumer.ChainComplete(status & ~ChainStatus.Flow);
                }
                finally
                {
                    consumer.ChainDispose();
                }
            }
            else
            {
                Cistern.Linq.Consume.ReadOnlySpan.Invoke(Underlying, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);
            }
        }

        internal override ConsumableEnumerator<U> Clone() =>
            new WhereSelectArray<T, U>(Underlying, Predicate, Selector);

        public override bool MoveNext()
        {
            if (_state != 1)
                return false;

            while (_idx < Underlying.Length)
            {
                var current = Underlying[_idx++];
                if (Predicate(current))
                {
                    _current = Selector(current);
                    return true;
                }
            }

            _current = default(U);
            return false;
        }

        public override object TailLink => this; // for IMergeSelect<U>

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) =>
            throw new NotImplementedException();

        public override IConsumable<U> AddTail(ILink<U, U> transform) =>
            new Array<T, U>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public override IConsumable<V> AddTail<V>(ILink<U, V> transform) =>
            new Array<T, V>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        IConsumable<V> Optimizations.IMergeSelect<U>.MergeSelect<V>(IConsumable<U> _, Func<U, V> selector) =>
            new WhereSelectArray<T, V>(Underlying, Predicate, x => selector(Selector(x)));
    }

    partial class WhereSelectEnumerable<TEnumerable, TEnumerator, T, U>
        : ConsumableEnumerator<U>
        , Optimizations.IMergeSelect<U>
        where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
        where TEnumerator : IEnumerator<T>
    {
        internal TEnumerable Underlying { get; }
        internal Func<T, bool> Predicate { get; }
        internal Func<T, U> Selector { get; }

        TEnumerator _enumerator;

        public WhereSelectEnumerable(TEnumerable enumerable, Func<T, bool> predicate, Func<T, U> selector) =>
            (Underlying, Predicate, Selector) = (enumerable, predicate, selector);

        public override void Consume(Consumer<U> consumer)
        {
            if (consumer is Optimizations.ITailEnd<U> optimized)
            {
                try
                {
                    var status = ChainStatus.Flow;
                    if (Underlying.TryGetSourceAsSpan(out var span))
                        status = optimized.WhereSelect(span, Predicate, Selector);
                    else
                        status = optimized.WhereSelect<TEnumerable, TEnumerator, T>(Underlying, Predicate, Selector);

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
                    Cistern.Linq.Consume.ReadOnlySpan.Invoke(span, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);
                else
                    Cistern.Linq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T, U>(Underlying, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);
            }
        }

        internal override ConsumableEnumerator<U> Clone() =>
            new WhereSelectEnumerable<TEnumerable, TEnumerator, T, U>(Underlying, Predicate, Selector);

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
                        var current = _enumerator.Current;
                        if (Predicate(current))
                        {
                            _current = Selector(current);
                            return true;
                        }
                    }
                    _state = int.MaxValue;
                    goto default;

                default:
                    _current = default(U);
                    if (_enumerator != null)
                    {
                        _enumerator.Dispose();
                        _enumerator = default;
                    }
                    return false;
            }
        }

        public override object TailLink => this; // for IMergeSelect

        public override IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new NotImplementedException();

        public override IConsumable<U> AddTail(ILink<U, U> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, U>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public override IConsumable<V> AddTail<V>(ILink<U, V> transform) =>
            new Enumerable<TEnumerable, TEnumerator, T, V>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public virtual IConsumable<V> MergeSelect<V>(IConsumable<U> _, Func<U, V> selector) =>
            new WhereSelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, Predicate, x => selector(Selector(x)));
    }

    sealed class WhereSelectList<T, U>
        : WhereSelectEnumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T, U>
    {
        public WhereSelectList(List<T> list, Func<T, bool> predicate, Func<T, U> selector) 
            : base(new Optimizations.ListEnumerable<T>(list), predicate, selector) { }

        public override IConsumable<V> MergeSelect<V>(IConsumable<U> _, Func<U, V> selector) =>
            new WhereSelectList<T, V>(Underlying.List, Predicate, x => selector(Selector(x)));

        public override void Consume(Consumer<U> consumer)
        {
            if (consumer is Optimizations.IPipelineList<U> pipeline)
            {
                try
                {
                    var status = pipeline.WhereSelect(Underlying.List, Predicate, Selector);
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
