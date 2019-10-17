using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed class Empty<T> 
        : IConsumable<T>
        , IEnumerator<T>
        , Optimizations.IConsumableFastCount
    {
        public static IConsumable<T> Instance = new Empty<T>();

        private Empty() { }

        public IConsumable<W> Create<W>(ILink<T, W> first) => Empty<W>.Instance;

        public IConsumable<T> AddTail(ILink<T, T> transform) => this;
        public IConsumable<U> AddTail<U>(ILink<T, U> transform) => Empty<U>.Instance;

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Consume(Consumer<T> consumer)
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

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) => 0;
        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) => 0;
    }
}
