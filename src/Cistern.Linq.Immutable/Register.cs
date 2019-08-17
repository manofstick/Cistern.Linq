using Cistern.Linq.ChainLinq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cistern.Linq.Immutable
{
    public static class Register
    {
        class TryFindImmutableTypes
            : Utils.ITryFindSpecificType
        {
            private TryFindImmutableTypes() { }

            public static Utils.ITryFindSpecificType Instance { get; } = new TryFindImmutableTypes();

            public string Namespace => "System.Collections.Immutable";

            public Consumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : Utils.IConstruct<T, U>
            {
                if (name.Length <= 9)
                    return null;

                var ninthChar = name[9]; //  here     |
                                         //          \|/
                                         // 'ImmutableXXXX'
                                         //  0123456789012
                if (ninthChar == 'A' && e is ImmutableArray<T> a)      return construct.Create<ImmutableArrayEnumerable<T>,     ImmutableArrayEnumerator<T>>     (new ImmutableArrayEnumerable<T>(a));
                if (ninthChar == 'H' && e is ImmutableHashSet<T> hs)   return construct.Create<ImmutableHashSetEnumerable<T>,   ImmutableHashSet<T>.Enumerator>  (new ImmutableHashSetEnumerable<T>(hs));
                if (ninthChar == 'L' && e is ImmutableList<T> l)       return construct.Create<ImmutableListEnumerable<T>,      ImmutableList<T>.Enumerator>     (new ImmutableListEnumerable<T>(l));
                if (ninthChar == 'Q' && e is ImmutableQueue<T> q)      return construct.Create<ImmutableQueueEnumerable<T>,     ImmutableQueueEnumerator<T>>     (new ImmutableQueueEnumerable<T>(q));
                if (ninthChar == 'S' && e is ImmutableSortedSet<T> ss) return construct.Create<ImmutableSortedSetEnumerable<T>, ImmutableSortedSet<T>.Enumerator>(new ImmutableSortedSetEnumerable<T>(ss));
                if (ninthChar == 'S' && e is ImmutableStack<T> s)      return construct.Create<ImmutableStackEnumerable<T>,     ImmutableStackEnumerator<T>>     (new ImmutableStackEnumerable<T>(s));

                return null;
            }
        }

        public static void RegisterSystemCollectionsImmutable()
        {
            Utils.Register(TryFindImmutableTypes.Instance);
        }
    }
}
