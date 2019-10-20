using BenchmarkDotNet.Attributes;
using Cistern.Linq.Benchmarking.Data;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    public abstract class VanillaCustomerArrayBase
    {
        [Params(0, 1, 10, 1000)]
        public int CustomerCount;

        protected DummyData.Customer[] Customers;
        protected string[] States;

        [GlobalSetup]
        public void Setup()
        {
            Customers = DummyData.GetCustomers(CustomerCount).ToArray();
            States = Customers.Select(c => c.State).ToArray();
        }
    }
}
