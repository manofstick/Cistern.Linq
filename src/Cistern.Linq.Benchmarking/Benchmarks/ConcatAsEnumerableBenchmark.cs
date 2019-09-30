using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    /*
    |      Method | NumberOfConcats |         Mean |     Error |    StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |---------------- |-------------:|----------:|----------:|------:|--------:|------:|------:|----------:|
    |  SystemLinq |              10 |     1.673 us | 0.0028 us | 0.0025 us |  1.00 |  0.3109 |     - |     - |     976 B |
    | CisternLinq |              10 |     1.851 us | 0.0078 us | 0.0065 us |  1.11 |  0.4215 |     - |     - |    1328 B |
    |             |                 |              |           |           |       |         |       |       |           |
    |  SystemLinq |             100 |    24.681 us | 0.0267 us | 0.0237 us |  1.00 |  3.0518 |     - |     - |    9616 B |
    | CisternLinq |             100 |    16.336 us | 0.0333 us | 0.0311 us |  0.66 |  3.1128 |     - |     - |    9768 B |
    |             |                 |              |           |           |       |         |       |       |           |
    |  SystemLinq |            1000 | 1,501.272 us | 8.4368 us | 7.4790 us |  1.00 | 29.2969 |     - |     - |   96016 B |
    | CisternLinq |            1000 |   159.825 us | 0.3544 us | 0.3315 us |  0.11 | 28.3203 |     - |     - |   89072 B |
    */

    [CoreJob, MemoryDiagnoser]
	public class ConcatAsEnumerableBenchmark
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
                for (var j = 0; j < DataToConcat[i].Length; ++j)
                    DataToConcat[i][j] = (byte)j;
            }
		}

		[Benchmark(Baseline = true)]
		public int SystemLinq()
		{
			var enumerable = (IEnumerable<byte>)new byte[0];
			foreach (var array in DataToConcat)
			{
				enumerable = System.Linq.Enumerable.Concat(enumerable, array);
			}

            var sum = 0;
            foreach (var item in enumerable)
                sum += item;
			return sum;
		}
		
		[Benchmark]
		public int CisternLinq()
		{
			var enumerable = (IEnumerable<byte>)new byte[0];
			foreach (var array in DataToConcat)
			{
				enumerable = Enumerable.Concat(enumerable, array);
			}

            var sum = 0;
            foreach (var item in enumerable)
                sum += item;
            return sum;
        }
    }
}
