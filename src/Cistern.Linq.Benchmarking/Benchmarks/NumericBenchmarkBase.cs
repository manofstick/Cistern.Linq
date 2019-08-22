using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
	public abstract class NumericBenchmarkBase
	{

		[Params(10, 1000, 1000000)]
		public int NumberOfItems;

		public double[] Numbers;

		[GlobalSetup]
		public void Setup()
		{
			Numbers = new double[NumberOfItems];
			for (int i = 0; i < NumberOfItems; i++)
			{
				Numbers[i] = i + 1;
			}
		}
	}
}
