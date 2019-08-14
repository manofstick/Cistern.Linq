using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        public static IEnumerator<U> Get<T, U>(Consumables.Enumerable<T, U> consumable) =>
            new ConsumerEnumerators.Enumerable<T, U, Optimizations.IEnumerableEnumerable<T>, IEnumerator<T>>(new Optimizations.IEnumerableEnumerable<T>(consumable.Underlying), consumable.Link);
    }
}
