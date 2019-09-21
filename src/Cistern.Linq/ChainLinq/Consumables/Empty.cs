using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed class Empty<T> 
        : ConsumableCons<T>
        , IEnumerator<T>
        , Optimizations.ICountOnConsumable
    {
        public static Consumable<T> Instance = new Empty<T>();

        private Empty() { }

        public Consumable<W> Create<W>(ILink<T, W> first) => Empty<W>.Instance;

        public override Consumable<T> AddTail(ILink<T, T> transform) => this;
        public override Consumable<U> AddTail<U>(ILink<T, U> transform) => Empty<U>.Instance;

        public override IEnumerator<T> GetEnumerator() => this;

        public override void Consume(Consumer<T> consumer)
        {
            try
            {
                consumer.ChainComplete(ChainStatus.Filter);
            }
            finally
            {
                consumer.ChainDispose();
            }
        }

        void IDisposable.Dispose() { }
        bool IEnumerator.MoveNext() => false;
        void IEnumerator.Reset() { }

        
        object IEnumerator.Current => default;
        T IEnumerator<T>.Current => default;

        public override object TailLink => null;
        public override Consumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink) => throw new InvalidOperationException();

        int Optimizations.ICountOnConsumable.GetCount(bool onlyIfCheap) => 0;
    }
}
