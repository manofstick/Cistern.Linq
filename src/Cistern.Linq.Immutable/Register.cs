using Cistern.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cistern.Linq.Immutable
{
    public static class Register
    {
        class TryFindImmutableTypes
            : Registry.ITryFindSpecificType
        {
            private TryFindImmutableTypes() { }

            public static Registry.ITryFindSpecificType Instance { get; } = new TryFindImmutableTypes();

            public string Namespace => "System.Collections.Immutable";

            public IConsumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : Registry.IConstruct<T, U>
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

            public bool TryInvoke<T, Invoker>(Invoker invoker, IEnumerable<T> e, string name)
                where Invoker : Registry.IInvoker<T>
            {
                if (name.Length <= 9)
                    return false;

                var ninthChar = name[9]; //  here     |
                                         //          \|/
                                         // 'ImmutableXXXX'
                                         //  0123456789012
                if (ninthChar == 'A' && e is ImmutableArray<T> a)      { invoker.Invoke<ImmutableArrayEnumerable<T>,     ImmutableArrayEnumerator<T>>     (new ImmutableArrayEnumerable<T>(a));      return true; }
                if (ninthChar == 'H' && e is ImmutableHashSet<T> hs)   { invoker.Invoke<ImmutableHashSetEnumerable<T>,   ImmutableHashSet<T>.Enumerator>  (new ImmutableHashSetEnumerable<T>(hs));   return true; }
                if (ninthChar == 'L' && e is ImmutableList<T> l)       { invoker.Invoke<ImmutableListEnumerable<T>,      ImmutableList<T>.Enumerator>     (new ImmutableListEnumerable<T>(l));       return true; }
                if (ninthChar == 'Q' && e is ImmutableQueue<T> q)      { invoker.Invoke<ImmutableQueueEnumerable<T>,     ImmutableQueueEnumerator<T>>     (new ImmutableQueueEnumerable<T>(q));      return true; }
                if (ninthChar == 'S' && e is ImmutableSortedSet<T> ss) { invoker.Invoke<ImmutableSortedSetEnumerable<T>, ImmutableSortedSet<T>.Enumerator>(new ImmutableSortedSetEnumerable<T>(ss)); return true; }
                if (ninthChar == 'S' && e is ImmutableStack<T> s)      { invoker.Invoke<ImmutableStackEnumerable<T>,     ImmutableStackEnumerator<T>>     (new ImmutableStackEnumerable<T>(s));      return true; }

                return false;
            }
        }

        public static void RegisterSystemCollectionsImmutable()
        {
            Registry.Register(TryFindImmutableTypes.Instance);
        }
    }
}
