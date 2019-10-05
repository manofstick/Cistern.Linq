using Cistern.Linq;
using System.Collections.Generic;

namespace Cistern.Linq.Immutable
{
    public static class Register
    {
        class TryFindSystemCollectionsTypes
            : Registry.ITryFindSpecificType
        {
            private TryFindSystemCollectionsTypes() { }

            public static Registry.ITryFindSpecificType Instance { get; } = new TryFindSystemCollectionsTypes();

            public string Namespace => "System.Collections.Generic";

            public IConsumable<U> TryCreateSpecific<T, U, Construct>(Construct construct, IEnumerable<T> e, string name)
                where Construct : Registry.IConstruct<T, U>
            {
                var firstChar = name[0];

                if (firstChar == 'H' && e is HashSet<T> hs)    return construct.Create<HashSetEnumerable<T>,    HashSet<T>.Enumerator>   (new HashSetEnumerable<T>(hs));
                if (firstChar == 'S' && e is Stack<T> s)       return construct.Create<StackEnumerable<T>,      Stack<T>.Enumerator>     (new StackEnumerable<T>(s));
                if (firstChar == 'S' && e is SortedSet<T> ss)  return construct.Create<SortedSetEnumerable<T>,  SortedSet<T>.Enumerator> (new SortedSetEnumerable<T>(ss));
                if (firstChar == 'L' && e is LinkedList<T> ll) return construct.Create<LinkedListEnumerable<T>, LinkedList<T>.Enumerator>(new LinkedListEnumerable<T>(ll));
                if (firstChar == 'Q' && e is Queue<T> q)       return construct.Create<QueueEnumerable<T>,      Queue<T>.Enumerator>     (new QueueEnumerable<T>(q));

                return null;
            }

            public bool TryInvoke<T, Invoker>(Invoker invoker, IEnumerable<T> e, string name)
                where Invoker : Registry.IInvoker<T>
            {
                var firstChar = name[0];

                if (firstChar == 'H' && e is HashSet<T> hs)    { invoker.Invoke<HashSetEnumerable<T>,    HashSet<T>.Enumerator>   (new HashSetEnumerable<T>(hs));    return true; }
                if (firstChar == 'S' && e is Stack<T> s)       { invoker.Invoke<StackEnumerable<T>,      Stack<T>.Enumerator>     (new StackEnumerable<T>(s));       return true; }
                if (firstChar == 'S' && e is SortedSet<T> ss)  { invoker.Invoke<SortedSetEnumerable<T>,  SortedSet<T>.Enumerator> (new SortedSetEnumerable<T>(ss));  return true; }
                if (firstChar == 'L' && e is LinkedList<T> ll) { invoker.Invoke<LinkedListEnumerable<T>, LinkedList<T>.Enumerator>(new LinkedListEnumerable<T>(ll)); return true; }
                if (firstChar == 'Q' && e is Queue<T> q)       { invoker.Invoke<QueueEnumerable<T>,      Queue<T>.Enumerator>     (new QueueEnumerable<T>(q));       return true; }

                return false;
            }
        }

        public static void RegisterSystemCollections()
        {
            Registry.Register(TryFindSystemCollectionsTypes.Instance);
        }
    }
}
