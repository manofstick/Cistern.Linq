﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Consumables
{
    sealed class Empty<T> 
        : Consumable<T>
        , IEnumerator<T>
        , Optimizations.IConsumableFastCount
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

        int? Optimizations.IConsumableFastCount.TryFastCount(bool asCountConsumer) => 0;
        int? Optimizations.IConsumableFastCount.TryRawCount(bool asCountConsumer) => 0;
    }
}