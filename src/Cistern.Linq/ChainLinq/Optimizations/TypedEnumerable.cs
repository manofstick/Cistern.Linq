using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ITypedEnumerable<T, Enumerator>
        where Enumerator : IEnumerator<T>
    {
        Enumerator GetEnumerator();
    }

    struct IEnumerableEnumerable<T>
        : ITypedEnumerable<T, IEnumerator<T>>
    {
        private IEnumerable<T> source;
        public IEnumerableEnumerable(IEnumerable<T> source) => this.source = source;

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();
    }

    struct ListEnumerable<T>
        : ITypedEnumerable<T, List<T>.Enumerator>
    {
        private List<T> source;
        public ListEnumerable(List<T> source) => this.source = source;

        public List<T>.Enumerator GetEnumerator() => source.GetEnumerator();
    }
}
