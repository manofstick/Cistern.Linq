using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        public static IEnumerator<U> Get<T, U>(Consumables.List<T, U> consumable) =>
            new ConsumerEnumerators.Enumerable<T, U, Optimizations.ListEnumerable<T>, List<T>.Enumerator>(new Optimizations.ListEnumerable<T>(consumable.Underlying), consumable.Link);
    }
}
