﻿using System;
using System.Diagnostics;

namespace Cistern.Linq.Consumables
{
    // Grouping is a publically exposed class, so we provide this class get the Consumable
    [DebuggerDisplay("Key = {Key}")]
    [DebuggerTypeProxy(typeof(SystemLinq_GroupingDebugView<,>))]
    internal class GroupingInternal<TKey, TElement>
        : Grouping<TKey, TElement>
        , IConsumable<TElement>
    {
        internal GroupingInternal(Lookup<TKey, TElement> owner) : base(owner) {}

        public IConsumable<TElement> AddTail(ILink<TElement, TElement> transform) =>
            (_count == 1)
            ? (IConsumable<TElement>)new IList<TElement, TElement>(this, 0, 1, transform)
            : (IConsumable<TElement>)new Array<TElement, TElement>(_elementsOrNull, 0, _count, transform);

        public IConsumable<U> AddTail<U>(ILink<TElement, U> transform) =>
            (_count == 1)
            ? (IConsumable<U>)new IList<TElement, U>(this, 0, 1, transform)
            : (IConsumable<U>)new Array<TElement, U>(_elementsOrNull, 0, _count, transform);

        public void Consume(Consumer<TElement> consumer)
        {
            if (_count == 1)
            {
                Cistern.Linq.Consume.IList.Invoke(this, 0, 1, consumer);
            }
            else
            {
                Cistern.Linq.Consume.ReadOnlySpan.Invoke(new ReadOnlySpan<TElement>(_elementsOrNull, 0, _count), consumer);
            }
        }
    }
}
