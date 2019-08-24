using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    public enum ContainerType
    {
        Array,
        List,
        Enumerable,
        ImmutableArray,
        ImmutableList,
        FSharpList
    }

    public abstract class ContainersBase
    {
        [Params(ContainerType.Array, ContainerType.List, ContainerType.Enumerable, ContainerType.ImmutableArray, ContainerType.ImmutableList, ContainerType.FSharpList)]
        public ContainerType ContainerType;

        [Params(10, 1000, 100000)]
        public int CustomerCount;

        protected IEnumerable<DummyData.Customer> Customers;

        private static IEnumerable<T> AsRealEnumerable<T>(IEnumerable<T> data)
        {
            foreach (var x in data)
                yield return x;
        }

        IEnumerable<T> ToContainer<T>(IEnumerable<T> data, ContainerType type)
        {
            switch(type)
            {
                case ContainerType.Array:           return data.ToArray();
                case ContainerType.List:            return data.ToList();
                case ContainerType.Enumerable:      return AsRealEnumerable(data);
                case ContainerType.ImmutableArray:  return data.ToImmutableArray();
                case ContainerType.ImmutableList:   return data.ToImmutableList();
                case ContainerType.FSharpList:      return Microsoft.FSharp.Collections.ListModule.OfSeq(data);
            }
            throw new ArgumentException($"unknwon type {type}", "type");
        }

		[GlobalSetup]
		public void Setup()
		{
            Immutable.Register.RegisterSystemCollectionsImmutable();
            FSharp.Register.RegisterFSharpCollections();

            var customers = DummyData.GetCustomers(CustomerCount);
            Customers = ToContainer(customers, ContainerType);
        }
	}
}
