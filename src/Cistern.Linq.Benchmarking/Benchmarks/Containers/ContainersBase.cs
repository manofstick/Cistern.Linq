using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    public abstract class ContainersBase
    {
        [ParamsSource(nameof(ValuesForCustomers))]
        public object CustomersData { get; set; }

        public IEnumerable<DummyData.Customer> Customers => (IEnumerable<DummyData.Customer>)CustomersData;

        private static IEnumerable<T> AsRealEnumerable<T>(IEnumerable<T> data)
        {
            foreach (var x in data)
                yield return x;
        }

        public IEnumerable<IEnumerable<DummyData.Customer>> ValuesForCustomers => new IEnumerable<DummyData.Customer>[] {
            (IEnumerable<DummyData.Customer>)DummyData.GetCustomers().ToArray(),
            (IEnumerable<DummyData.Customer>)DummyData.GetCustomers().ToList(),
            (IEnumerable<DummyData.Customer>)AsRealEnumerable(DummyData.GetCustomers()),
            (IEnumerable<DummyData.Customer>)DummyData.GetCustomers().ToImmutableArray(),
            (IEnumerable<DummyData.Customer>)DummyData.GetCustomers().ToImmutableList(),
            (IEnumerable<DummyData.Customer>)Microsoft.FSharp.Collections.ListModule.OfSeq(DummyData.GetCustomers()),
        };

		[GlobalSetup]
		public void Setup()
		{
            Immutable.Register.RegisterSystemCollectionsImmutable();
            FSharp.Register.RegisterFSharpCollections();
        }
	}
}
