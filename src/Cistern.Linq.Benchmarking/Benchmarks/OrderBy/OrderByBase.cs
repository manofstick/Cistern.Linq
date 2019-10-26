using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    public abstract class OrderByBase
    {
        [Params(0, 1, 10, 100, 1000, 10000)]
        public int CustomerCount;

        protected DummyData.Customer[] _customers;
        protected IEnumerable<DummyData.Customer> Customers
        {
            get
            {
                foreach (var customer in _customers)
                    yield return customer;
            }
        }

        protected virtual void GlobalSetup() { }

        [GlobalSetup]
        public void Setup()
        {
            _customers = DummyData.GetCustomers(CustomerCount).ToArray();
            GlobalSetup();
        }
    }
}
