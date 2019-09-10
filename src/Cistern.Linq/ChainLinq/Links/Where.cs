using System;

namespace Cistern.Linq.ChainLinq.Links
{
    internal partial class Where<T> : Link<T, T>
    {
        public Func<T, bool> Predicate { get; }

        public Where(Func<T, bool> predicate) =>
            Predicate = predicate;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(Predicate, activity);

        sealed partial class Activity : Activity<T, T>
        {
            private readonly Func<T, bool> _predicate;

            public Activity(Func<T, bool> predicate, Chain<T> next) : base(next) =>
                _predicate = predicate;

            public override ChainStatus ProcessNext(T input) =>
                _predicate(input) ? Next(input) : ChainStatus.Filter;
        }
    }
}
