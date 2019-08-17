using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq
{
    static class Utils
    {
        internal interface ITryFindSpecificType
        {
            string Namespace { get; }

            Consumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : IConstruct<T, U>;
        }

        private static object sync = new object();
        private static (string Namespace, ITryFindSpecificType TryFind)[] finders = Array.Empty<(string, ITryFindSpecificType)>();

        internal static void Register(ITryFindSpecificType finder)
        {
            lock(sync)
            {
                foreach(var item in finders)
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

        internal interface IConstruct<T, U>
        {
            Consumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T>;
        }

        internal static Consumable<U> CreateConsumableSearch<T, U, Construct>(Construct construct, IEnumerable<T> e)
            where Construct : IConstruct<T, U>
        {
            var ty = e.GetType();
            var enumerableNamespace = ty.Namespace;
            var enumerableName = ty.Name;
            if (enumerableNamespace == "System.Collections.Generic")
            {
                var firstChar = enumerableName[0];
                if (firstChar == 'H' && e is HashSet<T> hs)    return construct.Create<Optimizations.HashSetEnumerable<T>,    HashSet<T>.Enumerator>   (new Optimizations.HashSetEnumerable<T>(hs));
                if (firstChar == 'S' && e is Stack<T> s)       return construct.Create<Optimizations.StackEnumerable<T>,      Stack<T>.Enumerator>     (new Optimizations.StackEnumerable<T>(s));
                if (firstChar == 'S' && e is SortedSet<T> ss)  return construct.Create<Optimizations.SortedSetEnumerable<T>,  SortedSet<T>.Enumerator> (new Optimizations.SortedSetEnumerable<T>(ss));
                if (firstChar == 'L' && e is LinkedList<T> ll) return construct.Create<Optimizations.LinkedListEnumerable<T>, LinkedList<T>.Enumerator>(new Optimizations.LinkedListEnumerable<T>(ll));
                if (firstChar == 'Q' && e is Queue<T> q)       return construct.Create<Optimizations.QueueEnumerable<T>,      Queue<T>.Enumerator>     (new Optimizations.QueueEnumerable<T>(q));
            }

            foreach (var search in finders)
            {
                if (enumerableNamespace.Equals(search.Namespace))
                {
                    var found = search.TryFind.TryCreateSpecific<T, U, Construct>(construct, e, enumerableName);
                    if (found != null)
                        return found;
                }
            }

            return construct.Create<Optimizations.IEnumerableEnumerable<T>, IEnumerator<T>>(new Optimizations.IEnumerableEnumerable<T>(e));
        }

        struct Construct<T, U>
            : IConstruct<T, U>
        {
            private readonly Link<T, U> link;

            public Construct(Link<T, U> link) => this.link = link;

            public Consumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.Enumerable<TEnumerable, TEnumerator, T, U>(e, link);
        }


        internal static Consumable<U> CreateConsumable<T, U>(IEnumerable<T> e, Link<T, U> transform)
        {
            if (e is T[] array)
            {
                return
                    array.Length == 0
                      ? Consumables.Empty<U>.Instance
                      : new Consumables.Array<T, U>(array, 0, array.Length, transform);
            }
            else if (e is List<T> list)
            {
                return new Consumables.List<T, U>(list, transform);
            }
            else if (e is Consumables.IConsumableProvider<T> provider)
            {
                return provider.GetConsumable(transform);
            }
            /*
             * I don't think we should use IList in the general case?
             * 
                        else if (e is IList<T> ilist)
                        {
                            return new Consumables.IList<T, U>(ilist, 0, ilist.Count, transform);
                        }
            */
            else
            {
                return CreateConsumableSearch<T, U, Construct<T, U>>(new Construct<T, U>(transform), e);
            }
        }

        struct ConstructWhere<T>
            : IConstruct<T, T>
        {
            private readonly Func<T, bool> predicate;

            public ConstructWhere(Func<T, bool> predicate) => this.predicate = predicate;

            public Consumable<T> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.WhereEnumerable<TEnumerable, TEnumerator, T>(e, predicate);
        }

        internal static Consumable<TSource> Where<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source is ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is Optimizations.IMergeWhere<TSource> optimization)
                {
                    return optimization.MergeWhere(consumable, predicate);
                }

                return consumable.AddTail(new Links.Where<TSource>(predicate));
            }
            else if (source is TSource[] array)
            {
                if (array.Length == 0)
                    return Consumables.Empty<TSource>.Instance;
                else
                    return new Consumables.WhereArray<TSource>(array, predicate);
            }
            else if (source is List<TSource> list)
            {
                if (list.Count == 0)
                    return Consumables.Empty<TSource>.Instance;
                else
                    return new Consumables.WhereEnumerable<Optimizations.ListEnumerable<TSource>, List<TSource>.Enumerator, TSource>(new Optimizations.ListEnumerable<TSource>(list), predicate);
            }
            else
            {
                return CreateConsumableSearch<TSource, TSource,ConstructWhere<TSource>>(new ConstructWhere<TSource>(predicate), source);
            }
        }

        struct ConstructSelect<T, U>
            : IConstruct<T, U>
        {
            private readonly Func<T, U> selector;

            public ConstructSelect(Func<T, U> selector) => this.selector = selector;

            public Consumable<U> Create<TEnumerable, TEnumerator>(TEnumerable e)
                where TEnumerable : Optimizations.ITypedEnumerable<T, TEnumerator>
                where TEnumerator : IEnumerator<T> => new Consumables.SelectEnumerable<TEnumerable, TEnumerator, T, U>(e, selector);
        }

        internal static Consumable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is Optimizations.IMergeSelect<TSource> optimization)
                {
                    return optimization.MergeSelect(consumable, selector);
                }

                return consumable.AddTail(new Links.Select<TSource, TResult>(selector));
            }
            else if (source is TSource[] array)
            {
                if (array.Length == 0)
                    return Consumables.Empty<TResult>.Instance;
                else
                    return new Consumables.SelectArray<TSource, TResult>(array, selector);
            }
            else if (source is List<TSource> list)
            {
                return new Consumables.SelectEnumerable<Optimizations.ListEnumerable<TSource>, List<TSource>.Enumerator, TSource, TResult>(new Optimizations.ListEnumerable<TSource>(list), selector);
            }
            else
            {
                return CreateConsumableSearch<TSource, TResult, ConstructSelect<TSource, TResult>>(new ConstructSelect<TSource, TResult>(selector), source);
            }
        }

        internal static Consumable<T> AsConsumable<T>(IEnumerable<T> e)
        {
            if (e is Consumable<T> c)
            {
                return c;
            }
            else
            {
                return CreateConsumable(e, Links.Identity<T>.Instance);
            }
        }

        // TTTransform is faster tahn TUTransform as AddTail version call can avoid
        // expensive JIT generic interface call
        internal static Consumable<T> PushTTTransform<T>(IEnumerable<T> e, Link<T, T> transform)
        {
            if (e is ConsumableForAddition<T> consumable)
            {
                return consumable.AddTail(transform);
            }
            else
            {
                return CreateConsumable(e, transform);
            }
        }

        // TUTrasform is more flexible but slower than TTTransform
        internal static Consumable<U> PushTUTransform<T, U>(IEnumerable<T> e, Link<T, U> transform)
        {
            if (e is ConsumableForAddition<T> consumable)
            {
                return consumable.AddTail(transform);
            }
            else
            {
                return CreateConsumable(e, transform);
            }
        }

        internal static Result Consume<T, Result>(IEnumerable<T> e, Consumer<T, Result> consumer)
        {
            if (e is Consumable<T> consumable)
            {
                consumable.Consume(consumer);
            }
            else if (e is T[] array)
            {
                ChainLinq.Consume.ReadOnlySpan.Invoke(array, Links.Identity<T>.Instance, consumer);
            }
            else if (e is List<T> list)
            {
                ChainLinq.Consume.List.Invoke(list, Links.Identity<T>.Instance, consumer);
            }
            else if (e is Consumables.IConsumableProvider<T> provider)
            {
                var c = provider.GetConsumable(Links.Identity<T>.Instance);
                c.Consume(consumer);
            }
            else
            {
                ChainLinq.Consume.Enumerable.Invoke<Optimizations.IEnumerableEnumerable<T>, IEnumerator<T>, T, T>(new Optimizations.IEnumerableEnumerable<T>(e), Links.Identity<T>.Instance, consumer);
            }

            return consumer.Result;
        }
    }
}
