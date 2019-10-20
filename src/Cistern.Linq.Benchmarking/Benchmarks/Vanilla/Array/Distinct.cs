using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Vanilla.Array
{
    /*
    |      Method | CustomerCount |         Mean |       Error |      StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |-------------- |-------------:|------------:|------------:|------:|-------:|------:|------:|----------:|
    |     ForLoop |             0 |     65.81 ns |   0.1890 ns |   0.1768 ns |  0.31 | 0.0560 |     - |     - |     176 B |
    |  SystemLinq |             0 |    211.84 ns |   0.3777 ns |   0.3154 ns |  1.00 | 0.0381 |     - |     - |     120 B |
    | CisternLinq |             0 |     77.42 ns |   0.1602 ns |   0.1499 ns |  0.37 |      - |     - |     - |         - |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |             1 |     95.45 ns |   0.4063 ns |   0.3602 ns |  0.32 | 0.0663 |     - |     - |     208 B |
    |  SystemLinq |             1 |    302.79 ns |   1.1314 ns |   1.0029 ns |  1.00 | 0.1249 |     - |     - |     392 B |
    | CisternLinq |             1 |    162.53 ns |   0.2947 ns |   0.2757 ns |  0.54 | 0.1070 |     - |     - |     336 B |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |            10 |    384.86 ns |   0.3691 ns |   0.3082 ns |  0.60 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |            10 |    639.47 ns |  10.4155 ns |   9.7427 ns |  1.00 | 0.1249 |     - |     - |     392 B |
    | CisternLinq |            10 |    411.52 ns |   0.5473 ns |   0.5119 ns |  0.64 | 0.1068 |     - |     - |     336 B |
    |             |               |              |             |             |       |        |       |       |           |
    |     ForLoop |          1000 | 36,015.09 ns |  63.9501 ns |  53.4012 ns |  0.92 | 7.1411 |     - |     - |   22600 B |
    |  SystemLinq |          1000 | 39,045.08 ns | 125.6204 ns | 111.3592 ns |  1.00 | 0.1831 |     - |     - |     744 B |
    | CisternLinq |          1000 | 25,787.84 ns |  48.9294 ns |  38.2009 ns |  0.66 | 0.2136 |     - |     - |     688 B |
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
