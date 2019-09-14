using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq
{
    abstract class Chain
    {
        public abstract void ChainComplete();
        public abstract void ChainDispose();
    }

    [Flags]
    enum ChainStatus
    {
        /// <summary>
        /// Filter should not be used a flag, rather Flow flag not set
        /// </summary>
        Filter = 0x00,
        Flow = 0x01,
        Stop = 0x02,
    }

    static class ProcessNextResultHelper
    {
        public static bool IsStopped(this ChainStatus result) =>
            (result & ChainStatus.Stop) == ChainStatus.Stop;

        public static bool IsFlowing(this ChainStatus result) =>
            (result & ChainStatus.Flow) == ChainStatus.Flow;
    }

    abstract class Chain<T> : Chain
    {
        public abstract ChainStatus ProcessNext(T input);
    }

    interface ILink<T, U>
    {
        Chain<T> Compose(Chain<U> activity);
    }

    abstract class ActivityLink<T, U> : Chain<T>, ILink<T, U>
    {
        internal Chain<U> next;

        readonly int _threadId;
        protected bool _used;

        protected ActivityLink() => (_used, _threadId) = (false, Environment.CurrentManagedThreadId);

        protected ChainStatus Next(U u) => next.ProcessNext(u);

        public override void ChainComplete() => next.ChainComplete();
        public override void ChainDispose() => next.ChainDispose();

        public Chain<T> Compose(Chain<U> activity)
        {
            var chainT = (_threadId == Environment.CurrentManagedThreadId && !_used) ? this : Clone();
            chainT._used = true;
            chainT.next = activity;
            return chainT;
        }
        internal abstract ActivityLink<T, U> Clone();
    }


    abstract class Activity<T, U> : Chain<T>
    {
        readonly internal Chain<U> next;

        protected Activity(Chain<U> next) =>
            this.next = next;

        protected ChainStatus Next(U u) =>
            next.ProcessNext(u);

        public override void ChainComplete() => next.ChainComplete();
        public override void ChainDispose() => next.ChainDispose();
    }

    sealed class ChainEnd { private ChainEnd() { } }

    abstract class Consumer<T> : Chain<T>
    {
        public override void ChainComplete() { }
        public override void ChainDispose() { }
    }

    abstract class Consumer<T, R> : Consumer<T>
    {
        protected Consumer(R initalResult) =>
            Result = initalResult;

        public R Result { get; protected set; }
    }

    internal abstract class Consumable<T> : IEnumerable<T>
    {
        public abstract void Consume(Consumer<T> consumer);

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal abstract class ConsumableCons<T> : Consumable<T>
    {
        public abstract object TailLink { get; }
        public abstract Consumable<T> AddTail(ILink<T, T> transform);
        public abstract Consumable<U> AddTail<U>(ILink<T, U> transform);
        public abstract Consumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink);
    }
}
