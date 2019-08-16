using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Cistern.Linq.ChainLinq;

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
                if (name.Length < 9)
                    return null;

                var firstChar = name[9];
                if (firstChar == 'A' && e is ImmutableArray<T> a) return construct.Create<ImmutableArrayEnumerable<T>, ImmutableArrayEnumerator<T>>(new ImmutableArrayEnumerable<T>(a));

                return null;
            }
        }

        public static void RegisterSystemCollectionsImmutable()
        {
            Utils.Register(TryFindImmutableTypes.Instance);
        }
    }
}
