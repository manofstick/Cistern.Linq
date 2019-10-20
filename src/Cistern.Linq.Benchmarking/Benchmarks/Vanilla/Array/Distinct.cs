using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | CustomerCount |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     66.01 ns |   0.4204 ns |   0.3282 ns |  0.30 | 0.0560 |     - |     - |     176 B |
    |  SystemLinq |             0 |    217.84 ns |   0.4759 ns |   0.4452 ns |  1.00 | 0.0381 |     - |     - |     120 B |
    | CisternLinq |             0 |     75.41 ns |   0.3030 ns |   0.2686 ns |  0.35 |      - |     - |     - |         - |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |             1 |     95.36 ns |   0.3651 ns |   0.3237 ns |  0.32 | 0.0663 |     - |     - |     208 B |
    |  SystemLinq |             1 |    298.53 ns |   1.1158 ns |   1.0437 ns |  1.00 | 0.1249 |     - |     - |     392 B |
    | CisternLinq |             1 |    164.33 ns |   0.3386 ns |   0.3001 ns |  0.55 | 0.1070 |     - |     - |     336 B |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |            10 |    377.22 ns |   1.2692 ns |   1.0598 ns |  0.60 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |            10 |    631.67 ns |   3.9070 ns |   3.4634 ns |  1.00 | 0.1249 |     - |     - |     392 B |
    | CisternLinq |            10 |    407.13 ns |   0.7003 ns |   0.6550 ns |  0.64 | 0.1068 |     - |     - |     336 B |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |          1000 | 36,056.91 ns | 108.7229 ns | 101.6995 ns |  0.93 | 7.1411 |     - |     - |   22600 B |
    |  SystemLinq |          1000 | 38,697.31 ns | 118.7209 ns | 105.2430 ns |  1.00 | 0.1831 |     - |     - |     744 B |
    | CisternLinq |          1000 | 29,102.67 ns |  43.3426 ns |  38.4221 ns |  0.75 | 0.2136 |     - |     - |     688 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class VanillaArray_Distinct : VanillaCustomerArrayBase
    {
		[Benchmark]
		public double ForLoop() => (new HashSet<string>(States)).Count;

		[Benchmark(Baseline = true)]
		public int SystemLinq() => System.Linq.Enumerable.Distinct(States).Count();
		
		[Benchmark]
		public int CisternLinq() => Cistern.Linq.Enumerable.Distinct(States).Count();
    }
}
