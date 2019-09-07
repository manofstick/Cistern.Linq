namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ICountOnConsumable
    {
        int GetCount(bool onlyIfCheap);
    }

    // TODO: to support compose, this should be a two phase get count to ensure that the underlying links
    // both support counting. *Possibly upgrade this to just being part of the link.*
    interface ICountOnConsumableLink
    {
        int GetCount(int count);
    }
}
