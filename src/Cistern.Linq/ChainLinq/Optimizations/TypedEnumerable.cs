using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ITypedEnumerable<T, Enumerator>
        where Enumerator : IEnumerator<T>
    {
        IEnumerable<T> Source { get; }
        Enumerator GetEnumerator();
    }

    struct IEnumerableEnumerable<T>
        : ITypedEnumerable<T, IEnumerator<T>>
    {
        public IEnumerableEnumerable(IEnumerable<T> source) => Source = source;

        public IEnumerable<T> Source { get; }

        public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();
    }

    struct ListEnumerable<T>
        : ITypedEnumerable<T, List<T>.Enumerator>
    {
        private List<T> source;
        public ListEnumerable(List<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public List<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }

    struct LinkedListEnumerable<T>
        : ITypedEnumerable<T, LinkedList<T>.Enumerator>
    {
        private LinkedList<T> source;
        public LinkedListEnumerable(LinkedList<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public LinkedList<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }

    struct HashSetEnumerable<T>
        : ITypedEnumerable<T, HashSet<T>.Enumerator>
    {
        private HashSet<T> source;
        public HashSetEnumerable(HashSet<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public HashSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }

    struct StackEnumerable<T>
        : ITypedEnumerable<T, Stack<T>.Enumerator>
    {
        private Stack<T> source;
        public StackEnumerable(Stack<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public Stack<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }

    struct QueueEnumerable<T>
        : ITypedEnumerable<T, Queue<T>.Enumerator>
    {
        private Queue<T> source;
        public QueueEnumerable(Queue<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public Queue<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }

    struct SortedSetEnumerable<T>
        : ITypedEnumerable<T, SortedSet<T>.Enumerator>
    {
        private SortedSet<T> source;
        public SortedSetEnumerable(SortedSet<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public SortedSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }
}
