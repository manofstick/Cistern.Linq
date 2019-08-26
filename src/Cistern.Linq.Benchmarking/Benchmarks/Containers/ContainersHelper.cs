using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers
{
    static class ContainersHelper
    {
        private static IEnumerable<T> AsRealEnumerable<T>(T[] data)
        {
            foreach (var x in data)
                yield return x;
        }

        public static IEnumerable<T> ToContainer<T>(this IEnumerable<T> data, ContainerType type)
        {
            switch (type)
            {
                case ContainerType.Array:           return data.ToArray();
                case ContainerType.List:            return data.ToList();
                case ContainerType.Enumerable:      return AsRealEnumerable(data.ToArray());
                case ContainerType.ImmutableArray:  return data.ToImmutableArray();
                case ContainerType.ImmutableList:   return data.ToImmutableList();
                case ContainerType.FSharpList:      return Microsoft.FSharp.Collections.ListModule.OfSeq(data);
            }
            throw new ArgumentException($"unknwon type {type}", "type");
        }
    }
}
