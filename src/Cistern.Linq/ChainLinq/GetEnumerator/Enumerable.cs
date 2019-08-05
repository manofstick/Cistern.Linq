using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        public static IEnumerator<U> Get<T, U>(Consumables.Enumerable<T, U> consumable)
        {
            return new ConsumerEnumerators.Enumerable<T, U>(consumable.Underlying, consumable.Link);
        }
    }
}
