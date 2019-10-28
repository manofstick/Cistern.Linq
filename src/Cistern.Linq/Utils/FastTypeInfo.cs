namespace Cistern.Linq.UtilsTmp
{
    static class FastTypeInfo<T>
    {
        static private bool? isValueType;
        static public bool IsValueType => isValueType ??= typeof(T).IsValueType;
    }
}
