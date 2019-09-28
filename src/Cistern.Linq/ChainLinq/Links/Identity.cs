namespace Cistern.Linq.ChainLinq.Links
{
    sealed partial class Identity<T>
        : ILink<T, T>
        , Optimizations.ILinkFastCount
    {
        public static ILink<T, T> Instance { get; } = new Identity<T>();

        private Identity() { }

        Chain<T> ILink<T,T>.Compose(Chain<T> next) => next;

        bool Optimizations.ILinkFastCount.SupportedAsConsumer => true;
        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count) => count;
    }
}
