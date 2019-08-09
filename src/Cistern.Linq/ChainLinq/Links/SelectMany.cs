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
            , Optimizations.IHeadStart<T>
        {
            private readonly Func<T, IEnumerable<U>> collectionSelector;

            public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, IEnumerable<U>> collectionSelector) : base(next) =>
                this.collectionSelector = collectionSelector;

            public override ChainStatus ProcessNext(T input) =>
                Next((input, collectionSelector(input)));

            void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                foreach (var item in source)
                    Next((item, collectionSelector(item)));
            }

            void Optimizations.IHeadStart<T>.Execute(List<T> source)
            {
                foreach (var item in source)
                    Next((item, collectionSelector(item)));
            }

            void Optimizations.IHeadStart<T>.Execute(IList<T> source, int start, int count)
            {
                for (var i = start; i < start + count; ++i)
                {
                    var item = source[i];
                    Next((item, collectionSelector(item)));
                }
            }

            void Optimizations.IHeadStart<T>.Execute(IEnumerable<T> source)
            {
                foreach (var item in source)
                    Next((item, collectionSelector(item)));
            }
        }
    }
}
