using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed class SelectManyCount<Enumerable, T> : Consumer<Enumerable, int>
        where Enumerable : IEnumerable<T>
    {
        public SelectManyCount() : base(0) { }

        public override ChainStatus ProcessNext(Enumerable input)
        {
            checked
            {
                Result += input.Count();
            }
            return ChainStatus.Flow;
        }
    }

    sealed partial class SelectMany<Enumerable, T, V>
        : Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap)
        {
            if (onlyIfCheap)
            {
                return -1;
            }

            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                var selectManyCount = new SelectManyCount<Enumerable, T>();
                _selectMany.Consume(selectManyCount);
                var underlyingCount = selectManyCount.Result;

                var c = countLink.GetCount(underlyingCount);
                if (underlyingCount >= 0)
                    return underlyingCount;
            }

            var counter = new Consumer.Count<V>();
            Consume(counter);
            return counter.Result;
        }
    }
}
