using System.Collections.Generic;

namespace Cistern.Linq.GetEnumerator
{
    static class Range
    {
        public static IEnumerator<U> Get<U>(int start, int count, ILink<int, U> link)
        {
            return new ConsumerEnumerators.Range<U>(start, count, link);
        }
    }
}
