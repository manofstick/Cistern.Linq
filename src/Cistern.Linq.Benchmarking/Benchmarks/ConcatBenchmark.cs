using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
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
