using System.Collections.Generic;

namespace Cistern.Linq.Links
{
    sealed class Distinct<T>
        : ILink<T, T>
    {
        private readonly IEqualityComparer<T> comparer;

        public Distinct(IEqualityComparer<T> comparer) =>
            this.comparer = comparer;

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(comparer, activity);

        sealed class Activity : Activity<T, T>
        {
            private Set<T> _seen;

            public Activity(IEqualityComparer<T> comparer, Chain<T> next) : base(next) =>
                _seen = new Set<T>(comparer);

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

    sealed class DistinctDefaultComparer<T>
        : ILink<T, T>
    {
        public static readonly ILink<T, T> Instance = new DistinctDefaultComparer<T>();

        private DistinctDefaultComparer() { }

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(activity);

        sealed class Activity : Activity<T, T>
        {
            private SetDefaultComparer<T> _seen;

            public Activity(Chain<T> next) : base(next) =>
                _seen = new SetDefaultComparer<T>();

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }
}
