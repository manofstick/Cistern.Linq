using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq
{
    abstract class Chain
    {
        public abstract ChainStatus ChainComplete(ChainStatus status);
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

        public static bool NotStoppedAndFlowing(this ChainStatus result) =>
            result.IsFlowing() && !result.IsStopped();
    }

    abstract class Chain<T> : Chain
    {
        public abstract ChainStatus ProcessNext(T input);
    }

    interface ILink<T, U>
    {
        Chain<T> Compose(Chain<U> activity);
    }

    abstract class Activity<T, U> : Chain<T>
    {
        readonly internal Chain<U> next;

        protected Activity(Chain<U> next) =>
            this.next = next;

        protected ChainStatus Next(U u) =>
            next.ProcessNext(u);

        public override ChainStatus ChainComplete(ChainStatus status) => next.ChainComplete(status);
        public override void ChainDispose() => next.ChainDispose();
    }

    sealed class ChainEnd { private ChainEnd() { } }

    abstract class Consumer<T> : Chain<T>
    {
        public override ChainStatus ChainComplete(ChainStatus status) => ChainStatus.Stop;
        public override void ChainDispose() { }
    }

    abstract class Consumer<T, R> : Consumer<T>
    {
        protected Consumer(R initalResult) =>
            Result = initalResult;

        public R Result { get; protected set; }
    }

    static class Cache<T> { public static T Item; }

    internal interface IConsumable<T> : IEnumerable<T>
    {
        void Consume(Consumer<T> consumer);

        object TailLink { get; }
        IConsumable<T> AddTail(ILink<T, T> transform);
        IConsumable<U> AddTail<U>(ILink<T, U> transform);
        IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink);
    }

    internal abstract class Consumable<T> : IConsumable<T>
    {
        public abstract void Consume(Consumer<T> consumer);

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract object TailLink { get; }
        public abstract IConsumable<T> AddTail(ILink<T, T> transform);
        public abstract IConsumable<U> AddTail<U>(ILink<T, U> transform);
        public abstract IConsumable<V> ReplaceTailLink<Unknown, V>(ILink<Unknown, V> newLink);
    }
}
