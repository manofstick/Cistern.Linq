using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class SelectMany<T, U> : Link<T, (T, IEnumerable<U>)>
    {
        private readonly Func<T, IEnumerable<U>> collectionSelector;

        public SelectMany(Func<T, IEnumerable<U>> collectionSelector) : base(LinkType.SelectMany) =>
            this.collectionSelector = collectionSelector;

        public override Chain<T> Compose(Chain<(T, IEnumerable<U>)> next) =>
            new Activity(next, collectionSelector);

        private sealed class Activity
            : Activity<T, (T, IEnumerable<U>)>
            , Optimizations.IPipeline<ReadOnlyMemory<T>>
        {
            private readonly Func<T, IEnumerable<U>> collectionSelector;

            public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, IEnumerable<U>> collectionSelector) : base(next) =>
                this.collectionSelector = collectionSelector;

            public void Pipeline(ReadOnlyMemory<T> source)
            {
                foreach (var item in source.Span)
                    Next((item, collectionSelector(item)));
            }

            public override ChainStatus ProcessNext(T input) =>
                Next((input, collectionSelector(input)));
        }
    }
}
