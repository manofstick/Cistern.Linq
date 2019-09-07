using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    /*
    |      Method | NumberOfConcats |         Mean |      Error |     StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |---------------- |-------------:|-----------:|-----------:|------:|--------:|------:|------:|----------:|
    |     ForLoop |              10 |     659.4 ns |   4.808 ns |   4.498 ns |  0.56 |  0.1926 |     - |     - |     608 B |
    |  SystemLinq |              10 |   1,169.6 ns |  10.977 ns |  10.268 ns |  1.00 |  0.2480 |     - |     - |     784 B |
    | CisternLinq |              10 |   1,360.7 ns |   7.076 ns |   6.619 ns |  1.16 |  0.3338 |     - |     - |    1056 B |
    |             |                 |              |            |            |       |         |       |       |           |
    |     ForLoop |             100 |   4,738.6 ns |  32.913 ns |  30.787 ns |  0.45 |  1.2054 |     - |     - |    3816 B |
    |  SystemLinq |             100 |  10,485.4 ns |  81.140 ns |  75.899 ns |  1.00 |  2.3499 |     - |     - |    7440 B |
    | CisternLinq |             100 |  10,083.4 ns |  68.322 ns |  63.908 ns |  0.96 |  2.3804 |     - |     - |    7512 B |
    |             |                 |              |            |            |       |         |       |       |           |
    |     ForLoop |            1000 |  43,885.9 ns | 300.627 ns | 266.498 ns |  0.43 |  9.7656 |     - |     - |   30808 B |
    |  SystemLinq |            1000 | 102,141.4 ns | 611.215 ns | 571.731 ns |  1.00 | 23.4375 |     - |     - |   74040 B |
    | CisternLinq |            1000 |  94,669.1 ns | 353.365 ns | 330.538 ns |  0.93 | 21.2402 |     - |     - |   67016 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class ConcatBenchmark
	{
		[Params(10, 100, 1000)]
		public int NumberOfConcats;

		public byte[][] DataToConcat;

		public const int ArraySize = 10;

		[GlobalSetup]
		public void Setup()
		{
			DataToConcat = new byte[NumberOfConcats][];
			
			for (int i = 0; i < NumberOfConcats; i++)
			{
				DataToConcat[i] = new byte[ArraySize];
			}
		}

		[Benchmark]
		public byte[] ForLoop()
		{
			var items = new List<byte>();
			
			foreach (var array in DataToConcat)
			{
				items.AddRange(array);
			}
			
			return items.ToArray();
		}

		[Benchmark(Baseline = true)]
		public byte[] SystemLinq()
		{
			var enumerable = (IEnumerable<byte>)new byte[0];
			foreach (var array in DataToConcat)
			{
				enumerable = System.Linq.Enumerable.Concat(enumerable, array);
			}

			return System.Linq.Enumerable.ToArray(enumerable);
		}
		
		[Benchmark]
		public byte[] CisternLinq()
		{
			var enumerable = (IEnumerable<byte>)new byte[0];
			foreach (var array in DataToConcat)
			{
				enumerable = Enumerable.Concat(enumerable, array);
			}

			return Enumerable.ToArray(enumerable);
		}
	}
}
