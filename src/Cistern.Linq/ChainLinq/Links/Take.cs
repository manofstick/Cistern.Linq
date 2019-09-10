using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class Take<T>
        : Link<T, T>
        , Optimizations.ICountOnConsumableLink
    {
        private int _count;

        public Take(int count) =>
            _count = count;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(_count, activity);

        int Optimizations.ICountOnConsumableLink.GetCount(int count)
        {
            checked
            {
                return Math.Min(_count, count);
            }
        }

        sealed class Activity : Activity<T, T>
        {
            private readonly int count;

            private int index;

            public Activity(int count, Chain<T> next) : base(next) =>
                (this.count, index) = (count, 0);

            public override ChainStatus ProcessNext(T input)
            {
                if (index >= count)
                {
                    return ChainStatus.Stop;
                }

                checked
                {
                    index++;
                }

                if (index >= count)
                    return ChainStatus.Stop | Next(input);
                else
                    return Next(input);
            }
        }
    }
}
