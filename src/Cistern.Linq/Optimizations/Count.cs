namespace Cistern.Linq.Optimizations
{
    interface IConsumableFastCount
    {
        int? TryFastCount(bool asCountConsumer);
        int? TryRawCount(bool asCountConsumer);
    }

    interface ILinkFastCount
    {
        bool SupportedAsConsumer { get; }
        int? FastCountAdjustment(int count);
    }

    internal static class Count
    {
        public static int? TryGetCount(IConsumableFastCount c, object linkOrNull, bool asConsumer)
        {
            if (linkOrNull is null)
                return c.TryRawCount(asConsumer);

            if (linkOrNull is ILinkFastCount fast && (!asConsumer || fast.SupportedAsConsumer))
            {
                var rawCount = c.TryRawCount(asConsumer);
                if (rawCount.HasValue)
                    return fast.FastCountAdjustment(rawCount.Value);
            }

            return null;
        }
    }
}
