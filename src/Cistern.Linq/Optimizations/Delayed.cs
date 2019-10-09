namespace Cistern.Linq.Optimizations
{
    interface IDelayed<T>
    {
        IConsumable<T> Force();
    }
}
