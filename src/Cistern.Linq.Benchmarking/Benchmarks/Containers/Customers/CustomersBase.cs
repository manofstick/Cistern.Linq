using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    public abstract class CustomersBase
    {
        [Params(ContainerType.Array, ContainerType.List, ContainerType.Enumerable, ContainerType.ImmutableArray, ContainerType.ImmutableList, ContainerType.FSharpList)]
        public ContainerType ContainerType;

        [Params(0, 1, 10, 100, 1000)]
        public int CustomerCount;

        protected IEnumerable<DummyData.Customer> Customers;

		[GlobalSetup]
		public void Setup()
		{
            Cistern.Linq.Registry.Clear();
            switch(ContainerType)
            {
                case ContainerType.Array:
                case ContainerType.List:
                case ContainerType.Enumerable:
                    break;

                case ContainerType.ImmutableArray:
                case ContainerType.ImmutableList:
                    Immutable.Register.RegisterSystemCollectionsImmutable();
                    break;

                case ContainerType.FSharpList:
                    FSharp.Register.RegisterFSharpCollections();
                    break;
            }

            var customers = DummyData.GetCustomers(CustomerCount).OrderBy(x => x.State);
            Customers = ContainersHelper.ToContainer(customers, ContainerType);
        }
	}
}
