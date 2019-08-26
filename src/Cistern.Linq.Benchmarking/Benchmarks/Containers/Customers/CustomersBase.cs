using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    public abstract class CustomersBase
    {
        [Params(ContainerType.Array, ContainerType.List, ContainerType.Enumerable, ContainerType.ImmutableArray, ContainerType.ImmutableList, ContainerType.FSharpList)]
        public ContainerType ContainerType;

        [Params(10, 1000, 100000)]
        public int CustomerCount;

        protected IEnumerable<DummyData.Customer> Customers;

		[GlobalSetup]
		public void Setup()
		{
            Immutable.Register.RegisterSystemCollectionsImmutable();
            FSharp.Register.RegisterFSharpCollections();

            var customers = DummyData.GetCustomers(CustomerCount);
            Customers = ContainersHelper.ToContainer(customers, ContainerType);
        }
	}
}
