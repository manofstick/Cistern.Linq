using System;
using System.Collections.Generic;

namespace Cistern.Linq.Optimizations
{
    abstract class SimpleCollection<T, Enumerable>
        : ICollection<T>
        where Enumerable : IEnumerable<T>
    {
        protected Enumerable Parent { get; }
        internal SimpleCollection(Enumerable parent) => Parent = parent;

        public abstract int Count { get; }
        public abstract void CopyTo(T[] array, int arrayIndex);

        public bool IsReadOnly => true;

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Parent.GetEnumerator();
        public IEnumerator<T> GetEnumerator() => Parent.GetEnumerator();

        public bool Contains(T item) => Parent.Contains(item);

        public void Add(T item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Remove(T item) => throw new NotSupportedException();
    }

    interface ITryGetCollectionInterface<T>
    {
        bool TryGetCollectionInterface(out ICollection<T> collection);
    }
}
