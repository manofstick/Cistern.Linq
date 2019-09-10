namespace Cistern.Linq.ChainLinq.Links
{
    sealed partial class Identity<T>
        : Link<T, T>
        , Optimizations.ISkipTakeOnConsumableLinkUpdate<T, T>
        , Optimizations.ICountOnConsumableLink
    {
        public static Link<T, T> Instance { get; } = new Identity<T>();
        private Identity() { }

        public override Chain<T> Compose(Chain<T> next) => next;

        int Optimizations.ICountOnConsumableLink.GetCount(int count) => count;

        Link<T, T> Optimizations.ISkipTakeOnConsumableLinkUpdate<T, T>.Skip(int toSkip) => this;
    }
}
