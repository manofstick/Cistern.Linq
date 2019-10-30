using System.Runtime.CompilerServices;

namespace Cistern.Linq.UtilsTmp
{
    static class FastTypeInfo<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool IsValueType() => !(default(T) is null);
    }
}
