using System.Collections.Generic;

namespace Cistern.Linq.GetEnumerator
{
    static partial class Array
    {
        public static IEnumerator<U> Get<T, U>(T[] array, int start, int length, ILink<T, U> link)
        {
            return new ConsumerEnumerators.Array<T, U>(array, start, length, link);
        }
    }
}
