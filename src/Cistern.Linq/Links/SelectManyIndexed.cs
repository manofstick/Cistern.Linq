﻿using System;
using System.Collections.Generic;

namespace Cistern.Linq.Links
{
    sealed class SelectManyIndexed<T, U>
        : ILink<T, (T, IEnumerable<U>)>
    {
        private readonly Func<T, int, IEnumerable<U>> collectionSelector;

        public SelectManyIndexed(Func<T, int, IEnumerable<U>> collectionSelector) =>
            this.collectionSelector = collectionSelector;

        Chain<T> ILink<T, (T, IEnumerable<U>)>.Compose(Chain<(T, IEnumerable<U>)> next) =>
            new Activity(next, collectionSelector);

        private sealed class Activity : Activity<T, (T, IEnumerable<U>)>
        {
            private readonly Func<T, int, IEnumerable<U>> collectionSelector;
            private int index = 0;

            public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, int, IEnumerable<U>> collectionSelector) : base(next) =>
                this.collectionSelector = collectionSelector;

            public override ChainStatus ProcessNext(T input) =>
                Next((input, collectionSelector(input, index++)));
        }
    }
}
