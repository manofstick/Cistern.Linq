using Cistern.Linq.ChainLinq;
using System.Collections.Generic;

namespace Cistern.Linq.Immutable
{
    public static class Register
    {
        class TryFindSystemCollectionsTypes
            : ChainLinq.Utils.ITryFindSpecificType
        {
            private TryFindSystemCollectionsTypes() { }

            public static ChainLinq.Utils.ITryFindSpecificType Instance { get; } = new TryFindSystemCollectionsTypes();

            public string Namespace => "System.Collections.Generic";

            public Consumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : ChainLinq.Utils.IConstruct<T, U>
            {
                var firstChar = name[0];

                if (firstChar == 'H' && e is HashSet<T> hs)    return construct.Create<HashSetEnumerable<T>,    HashSet<T>.Enumerator>   (new HashSetEnumerable<T>(hs));
                if (firstChar == 'S' && e is Stack<T> s)       return construct.Create<StackEnumerable<T>,      Stack<T>.Enumerator>     (new StackEnumerable<T>(s));
                if (firstChar == 'S' && e is SortedSet<T> ss)  return construct.Create<SortedSetEnumerable<T>,  SortedSet<T>.Enumerator> (new SortedSetEnumerable<T>(ss));
                if (firstChar == 'L' && e is LinkedList<T> ll) return construct.Create<LinkedListEnumerable<T>, LinkedList<T>.Enumerator>(new LinkedListEnumerable<T>(ll));
                if (firstChar == 'Q' && e is Queue<T> q)       return construct.Create<QueueEnumerable<T>,      Queue<T>.Enumerator>     (new QueueEnumerable<T>(q));

                return null;
            }
        }

        public static void RegisterSystemCollections()
        {
            ChainLinq.Utils.Register(TryFindSystemCollectionsTypes.Instance);
        }
    }
}
