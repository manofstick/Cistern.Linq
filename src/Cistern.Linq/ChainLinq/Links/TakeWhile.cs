using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class TakeWhile<T>
        : ILink<T, T>
    {
        private readonly Func<T, bool> _predicate;

        public TakeWhile(Func<T, bool> predicate) =>
            _predicate = predicate;

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(_predicate, activity);

        sealed class Activity : Activity<T, T>
        {
            private readonly Func<T, bool> _predicate;

            public Activity(Func<T, bool> predicate, Chain<T> next) : base(next) =>
                _predicate = predicate;

            public override ChainStatus ProcessNext(T input)
            {
                if (_predicate(input))
                {
                    return Next(input);
                }
                return ChainStatus.Stop;
            }
        }
    }
}
