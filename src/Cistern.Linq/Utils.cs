using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static class Registry
    {
        internal interface IConstruct<T, U>
        {
            IConsumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T>;
        }

        internal interface IInvoker<T>
        {
            void Invoke<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T>;
        }

        internal interface ITryFindSpecificType
        {
            string Namespace { get; }

            IConsumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : IConstruct<T, U>;

            bool TryInvoke<T, Invoker>(Invoker invoker, IEnumerable<T> e, string name)
                where Invoker : IInvoker<T>;
        }

        private static object sync = new object();
        private static (string Namespace, ITryFindSpecificType TryFind)[] finders = Array.Empty<(string, ITryFindSpecificType)>();

        internal static void Register(ITryFindSpecificType finder)
        {
            lock (sync)
            {
                foreach (var item in finders)
                {
                    if (ReferenceEquals(item.TryFind, finder))
                        return;
                }

                var newArray = new (string, ITryFindSpecificType)[finders.Length + 1];
                finders.CopyTo(newArray, 0);
                newArray[newArray.Length - 1] = (finder.Namespace, finder);
                finders = newArray;
            }
        }

        internal static IConsumable<U> CreateConsumableSearch<T, U, Construct>(Construct construct, IEnumerable<T> e)
            where Construct : IConstruct<T, U>
        {
            if (finders.Length > 0)
            {
                var ty = e.GetType();

                var enumerableNamespace = ty.Namespace ?? String.Empty; // Namespace can be null - https://docs.microsoft.com/en-us/dotnet/api/system.type.namespace
                var enumerableName = ty.Name;

                foreach (var search in finders)
                {
                    if (enumerableNamespace.Equals(search.Namespace))
                    {
                        var found = search.TryFind.TryCreateSpecific<T, U, Construct>(construct, e, enumerableName);
                        if (found != null)
                            return found;
                    }
                }
            }

            return construct.Create<Optimizations.IEnumerableEnumerable<T>, IEnumerator<T>>(new Optimizations.IEnumerableEnumerable<T>(e));
        }

        internal static void InvokeSearch<T, Invoker>(Invoker invoker, IEnumerable<T> e)
            where Invoker : IInvoker<T>
        {
            if (finders.Length > 0)
            {
                var ty = e.GetType();

                var enumerableNamespace = ty.Namespace;
                var enumerableName = ty.Name;

                foreach (var search in finders)
                {
                    if (enumerableNamespace.Equals(search.Namespace))
                    {
                        var invoked = search.TryFind.TryInvoke(invoker, e, enumerableName);
                        if (invoked)
                            return;
                    }
                }
            }

            invoker.Invoke<Optimizations.IEnumerableEnumerable<T>, IEnumerator<T>>(new Optimizations.IEnumerableEnumerable<T>(e));
        }

        public static void Clear()
        {
            finders = Array.Empty<(string, ITryFindSpecificType)>();
        }
    }


    static class Utils
    {
        struct Construct<T, U>
            : Registry.IConstruct<T, U>
        {
            private readonly ILink<T, U> link;

            public Construct(ILink<T, U> link) => this.link = link;

            public IConsumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.Enumerable<TEnumerable, TEnumerator, T, U>(e, link);
        }


        internal static IConsumable<U> CreateConsumable<T, U>(IEnumerable<T> source, ILink<T, U> transform)
        {
            return source switch
            {
                T[] array                                   => ForArray(array, transform),
                List<T> list                                => ForList(list, transform),
                IConsumable<T> consumable                   => consumable.AddTail(transform),
                var e                                       => ForEnumerable(e, transform)
            };

            static IConsumable<U> ForArray(T[] array, ILink<T, U> transform) =>
                array.Length == 0
                    ? Consumables.Empty<U>.Instance
                    : new Consumables.Array<T, U>(array, 0, array.Length, transform);

            static IConsumable<U> ForList(List<T> list, ILink<T, U> transform) =>
                new Consumables.Enumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T, U>(new Optimizations.ListEnumerable<T>(list), transform);

            static IConsumable<U> ForEnumerable(IEnumerable<T> e, ILink<T, U> transform) =>
                Registry.CreateConsumableSearch<T, U, Construct<T, U>>(new Construct<T, U>(transform), e);
        }

        struct ConstructWhere<T>
            : Registry.IConstruct<T, T>
        {
            private readonly Func<T, bool> predicate;

            public ConstructWhere(Func<T, bool> predicate) => this.predicate = predicate;

            public IConsumable<T> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.WhereEnumerable<TEnumerable, TEnumerator, T>(e, predicate);
        }

        internal static IConsumable<TSource> Where<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source switch
            {
                Consumable<TSource> consumable  => ForConsumable(consumable, predicate),
                TSource[] array                 => ForArray(array, predicate),
                List<TSource> list              => ForList(list, predicate),
                IConsumable<TSource> consumable => ForIConsumable(consumable, predicate),
                var e                           => ForEnumerable(e, predicate)
            };

            static IConsumable<TSource> ForConsumable(Consumable<TSource> consumable, Func<TSource, bool> predicate)
            {
                if (consumable.TailLink is Optimizations.IMergeWhere<TSource> optimization)
                {
                    return optimization.MergeWhere(consumable, predicate);
                }
                return consumable.AddTail(new Links.Where<TSource>(predicate));
            }

            static IConsumable<TSource> ForArray(TSource[] array, Func<TSource, bool> predicate) =>
                (array.Length == 0)
                    ? Consumables.Empty<TSource>.Instance
                    : new Consumables.WhereArray<TSource>(array, predicate);

            static IConsumable<TSource> ForList(List<TSource> list, Func<TSource, bool> predicate) =>
                new Consumables.WhereList<TSource>(list, predicate);

            static IConsumable<TSource> ForIConsumable(IConsumable<TSource> consumable, Func<TSource, bool> predicate) =>
                consumable.AddTail(new Links.Where<TSource>(predicate));

            static IConsumable<TSource> ForEnumerable(IEnumerable<TSource> e, Func<TSource, bool> predicate) =>
                Registry.CreateConsumableSearch<TSource, TSource, ConstructWhere<TSource>>(new ConstructWhere<TSource>(predicate), e);
        }

        struct ConstructSelect<T, U>
            : Registry.IConstruct<T, U>
        {
            private readonly Func<T, U> selector;

            public ConstructSelect(Func<T, U> selector) => this.selector = selector;

            public IConsumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.SelectEnumerable<TEnumerable, TEnumerator, T, U>(e, selector);
        }

        internal static IConsumable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source switch
            {
                Consumable<TSource> consumable  => ForConsumable(consumable, selector),
                TSource[] array                 => ForArray(array, selector),
                List<TSource> list              => ForList(list, selector),
                IConsumable<TSource> consumable => ForIConsumable(consumable, selector),
                var e                           => ForEnumerable(e, selector),
            };

            static IConsumable<TResult> ForConsumable(Consumable<TSource> consumable, Func<TSource, TResult> selector)
            {
                if (consumable.TailLink is Optimizations.IMergeSelect<TSource> optimization)
                {
                    return optimization.MergeSelect(consumable, selector);
                }
                return consumable.AddTail(new Links.Select<TSource, TResult>(selector));
            }

            static IConsumable<TResult> ForArray(TSource[] array, Func<TSource, TResult> selector) =>
                array.Length == 0
                    ? Consumables.Empty<TResult>.Instance
                    : new Consumables.SelectArray<TSource, TResult>(array, selector);

            static IConsumable<TResult> ForList(List<TSource> list, Func<TSource, TResult> selector) =>
                new Consumables.SelectList<TSource, TResult>(list, selector);

            static IConsumable<TResult> ForIConsumable(IConsumable<TSource> consumable, Func<TSource, TResult> selector) =>
                consumable.AddTail(new Links.Select<TSource, TResult>(selector));

            static IConsumable<TResult> ForEnumerable(IEnumerable<TSource> source, Func<TSource, TResult> selector) =>
                Registry.CreateConsumableSearch<TSource, TResult, ConstructSelect<TSource, TResult>>(new ConstructSelect<TSource, TResult>(selector), source);
        }

        internal static ILink<T, T> GetSkipLink<T>(int skip) =>
            skip == 0 ? null : new Links.Skip<T>(skip);

        internal static IEnumerable<T> Skip<T>(IEnumerable<T> source, int skip)
        {
            return source switch
            {
                Consumable<T> consumable  => ForConsumable(consumable, skip),
                T[] array                 => ForArray(array, skip),
                List<T> list              => ForList(list, skip),
                IConsumable<T> consumable => ForIConsumable(consumable, skip),
                var e                     => ForEnumerable(e, skip),
            };

            static IConsumable<T> ForConsumable(Consumable<T> consumable, int skip)
            {
                if (consumable.TailLink is Optimizations.IMergeSkipTake<T> optimization)
                    return optimization.MergeSkip(consumable, skip);

                return consumable.AddTail(GetSkipLink<T>(skip));
            }

            static IConsumable<T> ForArray(T[] array, int skip)
            {
                var start = skip;
                var count = array.Length - skip;

                if (count <= 0)
                    return Consumables.Empty<T>.Instance;

                return new Consumables.Array<T, T>(array, start, count, null);
            }

            static IConsumable<T> ForList(List<T> list, int skip) =>
                new Consumables.Enumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T, T>(new Optimizations.ListEnumerable<T>(list), GetSkipLink<T>(skip));

            static IConsumable<T> ForIConsumable(IConsumable<T> consumable, int skip) =>
                consumable.AddTail(GetSkipLink<T>(skip));

            static IConsumable<T> ForEnumerable(IEnumerable<T> source, int skip) =>
                Registry.CreateConsumableSearch<T, T, Construct<T, T>>(new Construct<T, T>(GetSkipLink<T>(skip)), source);
        }

        internal static IEnumerable<T> Take<T>(IEnumerable<T> source, int take)
        {
            if (take <= 0)
                return Consumables.Empty<T>.Instance;

            return source switch
            {
                Consumable<T> consumable  => ForConsumable(consumable, take),
                T[] array                 => ForArray(array, take),
                List<T> list              => ForList(list, take),
                IConsumable<T> consumable => ForIConsumable(consumable, take),
                var e                     => ForEnumerable(e, take),
            };

            static IConsumable<T> ForConsumable(Consumable<T> consumable, int take)
            {
                if (consumable.TailLink is Optimizations.IMergeSkipTake<T> optimization)
                    return optimization.MergeTake(consumable, take);

                return consumable.AddTail(new Links.Take<T>(take));
            }

            static IConsumable<T> ForArray(T[] array, int take)
            {
                if (array.Length <= 0)
                    return Consumables.Empty<T>.Instance;

                return new Consumables.Array<T, T>(array, 0, Math.Min(take, array.Length), null);
            }

            static IConsumable<T> ForList(List<T> list, int take) =>
                new Consumables.Enumerable<Optimizations.ListEnumerable<T>, List<T>.Enumerator, T, T>(new Optimizations.ListEnumerable<T>(list), new Links.Take<T>(take));

            static IConsumable<T> ForIConsumable(IConsumable<T> consumable, int take) =>
                consumable.AddTail(new Links.Take<T>(take));

            static IConsumable<T> ForEnumerable(IEnumerable<T> source, int take) =>
                Registry.CreateConsumableSearch<T, T, Construct<T, T>>(new Construct<T, T>(new Links.Take<T>(take)), source);
        }

        internal static IConsumable<T> AsConsumable<T>(IEnumerable<T> e) =>
            e is Consumable<T> c ? c : CreateConsumable<T,T>(e, null);
            

        // TTTransform is faster tahn TUTransform as AddTail version call can avoid
        // expensive JIT generic interface call
        internal static IConsumable<T> PushTTTransform<T>(IEnumerable<T> e, ILink<T, T> transform) =>
            e is Consumable<T> c ? c.AddTail(transform) : CreateConsumable(e, transform);

        // TUTrasform is more flexible but slower than TTTransform
        internal static IConsumable<U> PushTUTransform<T, U>(IEnumerable<T> e, ILink<T, U> transform) =>
            e is Consumable<T> c ? c.AddTail(transform) : CreateConsumable(e, transform);

        internal static Result Consume<T, Result>(IConsumable<T> consumable, Consumer<T, Result> consumer)
        {
            consumable.Consume(consumer);
            return consumer.Result;
        }

        struct Invoker<T>
            : Registry.IInvoker<T>
        {
            private readonly Consumer<T> consumer;

            public Invoker(Consumer<T> consumer) => this.consumer = consumer;

            public void Invoke<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T>
            {
                if (e.TryGetSourceAsSpan(out var span))
                    Linq.Consume.ReadOnlySpan.Invoke(span, consumer);
                else
                    Linq.Consume.Enumerable.Invoke<TEnumerable, TEnumerator, T>(e, consumer);
            }
        }

        internal static Result Consume<T, Result>(IEnumerable<T> e, Consumer<T, Result> consumer)
        {
            switch (e)
            {
                case Consumable<T> consumable:
                    consumable.Consume(consumer);
                    break;

                case T[] array:
                    if (array.Length == 0)
                    {
                        try { consumer.ChainComplete(ChainStatus.Filter); }
                        finally { consumer.ChainDispose(); }
                    }
                    else
                    {
                        Linq.Consume.ReadOnlySpan.Invoke(array, consumer);
                    }
                    break;

                case List<T> list:
                    if (list.Count == 0)
                    {
                        try { consumer.ChainComplete(ChainStatus.Filter); }
                        finally { consumer.ChainDispose(); }
                    }
                    else
                    {
                        Linq.Consume.List.Invoke(list, consumer);
                    }
                    break;

                case IConsumable<T> consumable:
                    consumable.Consume(consumer);
                    break;

                default:
                    Registry.InvokeSearch(new Invoker<T>(consumer), e);
                    break;
            }

            return consumer.Result;
        }
    }
}
