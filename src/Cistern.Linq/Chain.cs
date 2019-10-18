using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

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

    static class Cache
    {
        public interface IClean : IDisposable
        {
            void Clean();
        }

        static class Typed<T> where T : class
        {
            public static T Item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGet<T>()
            where T : class =>
            Interlocked.Exchange(ref Typed<T>.Item, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stash<T>(T item)
            where T : class, IClean
        {
            item.Clean();
            Interlocked.MemoryBarrier();
            Typed<T>.Item = item;
        }
    }

    internal interface IConsumable<T> : IEnumerable<T>
    {
        void Consume(Consumer<T> consumer);

        IConsumable<T> AddTail(ILink<T, T> transform);
        IConsumable<U> AddTail<U>(ILink<T, U> transform);
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
