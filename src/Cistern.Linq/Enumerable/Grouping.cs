// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            new Consumables.GroupedEnumerable<TSource, TKey>(source, keySelector, null, false);

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
            new Consumables.GroupedEnumerable<TSource, TKey>(source, keySelector, comparer, false);

        internal static IEnumerable<IGrouping<TKey, TSource>> GroupByDelaySourceException<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
            new Consumables.GroupedEnumerable<TSource, TKey>(source, keySelector, comparer, true);

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
            new Consumables.GroupedEnumerable<TSource, TKey, TElement, IGrouping<TKey, TElement>>(source, keySelector, elementSelector, null, null, false);

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
            new Consumables.GroupedEnumerable<TSource, TKey, TElement, IGrouping<TKey, TElement>>(source, keySelector, elementSelector, comparer, null, false);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector) =>
            new Consumables.GroupedResultEnumerable<TSource, TKey, TSource, TResult, TResult>(source, keySelector, x => x, resultSelector, null, null);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
            new Consumables.GroupedResultEnumerable<TSource, TKey, TElement, TResult, TResult>(source, keySelector, elementSelector, resultSelector, null, null);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
            new Consumables.GroupedResultEnumerable<TSource, TKey, TSource, TResult, TResult>(source, keySelector, x=>x, resultSelector, comparer, null);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
            new Consumables.GroupedResultEnumerable<TSource, TKey, TElement, TResult, TResult>(source, keySelector, elementSelector, resultSelector, comparer, null);
    }

    public interface IGrouping<out TKey, out TElement> : IEnumerable<TElement>
    {
        TKey Key { get; }
    }

    internal sealed class GroupingArrayPool<TElement>
    {
        const int MinLength = 4; // relates to MinShift
        const int MinShift = 2; // relates to MinLength

        const int Buckets = 4;

        private (TElement[], TElement[]) _bucket_1;
        private (TElement[], TElement[]) _bucket_2;
        private (TElement[], TElement[]) _bucket_3;
        private (TElement[], TElement[]) _bucket_4;

        private GroupingArrayPool<TElement> _nextPool;
        private GroupingArrayPool<TElement> NextPool => _nextPool ?? (_nextPool = new GroupingArrayPool<TElement>());

        private static void TryPush(ref (TElement[], TElement[]) store, TElement[] toStore)
        {
            if (store.Item2 != null)
                return;

            Array.Clear(toStore, 0, toStore.Length);

            store.Item2 = store.Item1;
            store.Item1 = toStore;
        }

        private static TElement[] TryPop(ref (TElement[], TElement[]) store)
        {
            var head = store.Item1;

            if (head != null)
            {
                store.Item1 = store.Item2;
                store.Item2 = null;
            }

            return head;
        }

        private static TElement[] Upgrade(ref (TElement[], TElement[]) pushStore, ref (TElement[], TElement[]) popStore, TElement[] currentElements)
        {
            var newElements = TryPop(ref popStore);
            if (newElements == null)
            {
                newElements = new TElement[checked(currentElements.Length * 2)];
            }
            currentElements.CopyTo(newElements, 0);
            TryPush(ref pushStore, currentElements);
            return newElements;
        }

        private TElement[] FindBucketAndUpgrade(TElement[] currentElements, int shiftedLength)
        {
            if (shiftedLength <= 0x8)
            {
                switch (shiftedLength)
                {
                    case 1: return Upgrade(ref _bucket_1, ref _bucket_2, currentElements);
                    case 2: return Upgrade(ref _bucket_2, ref _bucket_3, currentElements);
                    case 4: return Upgrade(ref _bucket_3, ref _bucket_4, currentElements);
                    case 8: return Upgrade(ref _bucket_4, ref NextPool._bucket_1, currentElements);
                }
            }
            return NextPool.FindBucketAndUpgrade(currentElements, shiftedLength >> Buckets);
        }

        private static bool IsPowerOf2(int n) => n > 0 && (n & (n - 1)) == 0;

        public TElement[] Upgrade(TElement[] currentElements)
        {
            var length = currentElements.Length;

            Debug.Assert(IsPowerOf2(length), "Only powers of 2 lengths should be accepted");
            Debug.Assert(length >= MinLength, "Minimum size should be 4");

            var shiftedLength = length >> MinShift;

            return FindBucketAndUpgrade(currentElements, shiftedLength);
        }

        public TElement[] Alloc() => TryPop(ref _bucket_1) ?? new TElement[MinLength];
    }

    // It is (unfortunately) common to databind directly to Grouping.Key.
    // Because of this, we have to declare this internal type public so that we
    // can mark the Key property for public reflection.
    //
    // To limit the damage, the toolchain makes this type appear in a hidden assembly.
    // (This is also why it is no longer a nested type of Lookup<,>).
    [DebuggerDisplay("Key = {Key}")]
    [DebuggerTypeProxy(typeof(SystemLinq_GroupingDebugView<,>))]
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IList<TElement>
    {
        internal TKey _key;
        internal int _hashCode;

        Consumables.Lookup<TKey, TElement> _owner;
        internal int _count;
        /// <summary>
        /// for single elements buckets we don't allocate a seperate array, rather we use
        /// this slot to store the value. 
        /// NB. _element is only valid when _count = 1
        /// </summary>
        internal TElement _element;
        /// <summary>
        /// NB. _elementArray is not valid when _count = 1
        /// </summary>
        internal TElement[] _elementsOrNull;

        internal Consumables.GroupingInternal<TKey, TElement> _hashNext;
        internal Consumables.GroupingInternal<TKey, TElement> _next;

        internal Grouping(Consumables.Lookup<TKey, TElement> owner)
        {
            _owner = owner;
            _elementsOrNull = null;
        }

        internal void Add(TElement element)
        {
            if (_count == 0)
            {
                _element = element;
                _count = 1;
            }
            else
            {
                if (_count == 1)
                {
                    _elementsOrNull = _owner.Pool.Alloc();
                    _elementsOrNull[0] = _element;
                    _element = default(TElement);
                }

                if (_elementsOrNull.Length == _count)
                {
                    _elementsOrNull = _owner.Pool.Upgrade(_elementsOrNull);
                }

                _elementsOrNull[_count] = element;
                _count++;
            }
        }

        private void Trim()
        {
            if (_elementsOrNull != null && _elementsOrNull.Length != _count)
            {
                Array.Resize(ref _elementsOrNull, _count);
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            if (_count == 1)
            {
                yield return _element;
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {
                    yield return _elementsOrNull[i];
                }
            }
        }

        internal IList<TElement> GetEfficientList(bool canTrim)
        {
            if (_count <= 1 || (!canTrim && _count != _elementsOrNull.Length))
            {
                return this;
            }

            Trim();

            return _elementsOrNull;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // DDB195907: implement IGrouping<>.Key implicitly
        // so that WPF binding works on this property.
        public TKey Key => _key;

        int ICollection<TElement>.Count => _count;

        bool ICollection<TElement>.IsReadOnly => true;

        void ICollection<TElement>.Add(TElement item) => ThrowHelper.ThrowNotSupportedException();

        void ICollection<TElement>.Clear() => ThrowHelper.ThrowNotSupportedException();

        bool ICollection<TElement>.Contains(TElement item)
        {
            return _count switch
            {
                0 => false,
                1 => EqualityComparer<TElement>.Default.Equals(item, _element),
                _ => Array.IndexOf(_elementsOrNull, item, 0, _count) >= 0
            };
        }

        void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex)
        {
            switch (_count)
            {
                case 0:
                    break;

                case 1:
                    array[arrayIndex] = _element;
                    break;

                default:
                    Array.Copy(_elementsOrNull, 0, array, arrayIndex, _count);
                    break;
            }
        }

        bool ICollection<TElement>.Remove(TElement item)
        {
            ThrowHelper.ThrowNotSupportedException();
            return false;
        }

        int IList<TElement>.IndexOf(TElement item)
        {
            return _count switch
            {
                0 => -1,
                1 => EqualityComparer<TElement>.Default.Equals(item, _element) ? 0 : -1,
                _ => Array.IndexOf(_elementsOrNull, item, 0, _count),
            };
        }

        void IList<TElement>.Insert(int index, TElement item) => ThrowHelper.ThrowNotSupportedException();

        void IList<TElement>.RemoveAt(int index) => ThrowHelper.ThrowNotSupportedException();

        TElement IList<TElement>.this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
                }

                if (_count == 1)
                    return _element;

                return _elementsOrNull[index];
            }

            set
            {
                ThrowHelper.ThrowNotSupportedException();
            }
        }
    }
}
