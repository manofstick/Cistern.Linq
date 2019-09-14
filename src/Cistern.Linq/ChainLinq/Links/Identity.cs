namespace Cistern.Linq.ChainLinq.Links
{
    sealed partial class Identity<T>
        : ILink<T, T>
        , Optimizations.ICountOnConsumableLink
    {
        public static ILink<T, T> Instance { get; } = new Identity<T>();
        private Identity() { }

        Chain<T> ILink<T,T>.Compose(Chain<T> next) => next;

        int Optimizations.ICountOnConsumableLink.GetCount(int count) => count;
    }
}
