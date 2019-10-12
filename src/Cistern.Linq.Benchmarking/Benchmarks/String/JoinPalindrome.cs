using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
    |------------ |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|-----------:|----------:|----------:|-----------:|
    |     ForLoop |  False |         10 |         850.7 ns |         4.344 ns |         4.064 ns |  0.65 |    0.00 |     0.2623 |         - |         - |      824 B |
    |  SystemLinq |  False |         10 |       1,311.9 ns |         2.421 ns |         2.146 ns |  1.00 |    0.00 |     0.6084 |         - |         - |     1912 B |
    | CisternLinq |  False |         10 |       1,500.4 ns |         2.907 ns |         2.577 ns |  1.14 |    0.00 |     0.6332 |         - |         - |     1992 B |
    |             |        |            |                  |                  |                  |       |         |            |           |           |            |
    |     ForLoop |  False |       1000 |     106,819.6 ns |       187.067 ns |       165.830 ns |  0.62 |    0.00 |    20.8740 |         - |         - |    65872 B |
    |  SystemLinq |  False |       1000 |     172,368.4 ns |       805.894 ns |       714.404 ns |  1.00 |    0.00 |    47.1191 |    0.2441 |         - |   148464 B |
    | CisternLinq |  False |       1000 |     166,985.3 ns |     1,177.179 ns |     1,043.538 ns |  0.97 |    0.01 |    41.9922 |    0.2441 |         - |   132704 B |
    |             |        |            |                  |                  |                  |       |         |            |           |           |            |
    |     ForLoop |  False |     466544 | 136,230,634.6 ns |   924,100.584 ns |   771,665.841 ns |  0.39 |    0.01 |  7000.0000 |  500.0000 |  500.0000 | 20452520 B |
    |  SystemLinq |  False |     466544 | 347,080,280.8 ns | 6,886,979.154 ns | 9,426,974.803 ns |  1.00 |    0.00 | 11000.0000 | 4000.0000 | 1000.0000 | 61639848 B |
    | CisternLinq |  False |     466544 | 312,412,673.3 ns | 5,779,396.338 ns | 5,406,050.920 ns |  0.90 |    0.03 | 10000.0000 | 4000.0000 | 1000.0000 | 54175400 B |
    |             |        |            |                  |                  |                  |       |         |            |           |           |            |
    |     ForLoop |   True |         10 |         745.5 ns |         2.896 ns |         2.419 ns |  0.60 |    0.00 |     0.2289 |         - |         - |      720 B |
    |  SystemLinq |   True |         10 |       1,245.1 ns |         3.196 ns |         2.990 ns |  1.00 |    0.00 |     0.5760 |         - |         - |     1808 B |
    | CisternLinq |   True |         10 |       1,444.3 ns |         3.520 ns |         3.120 ns |  1.16 |    0.00 |     0.6008 |         - |         - |     1888 B |
    |             |        |            |                  |                  |                  |       |         |            |           |           |            |
    |     ForLoop |   True |       1000 |     100,565.3 ns |       289.342 ns |       256.494 ns |  0.61 |    0.01 |    19.8975 |         - |         - |    62888 B |
    |  SystemLinq |   True |       1000 |     164,062.8 ns |     1,786.788 ns |     1,583.941 ns |  1.00 |    0.00 |    45.8984 |    0.9766 |         - |   145480 B |
    | CisternLinq |   True |       1000 |     158,071.8 ns |       520.100 ns |       486.502 ns |  0.96 |    0.01 |    41.2598 |    0.4883 |         - |   129720 B |
    |             |        |            |                  |                  |                  |       |         |            |           |           |            |
    |     ForLoop |   True |     466544 |  99,952,832.0 ns | 1,333,269.373 ns | 1,247,140.999 ns |  0.33 |    0.01 |  6800.0000 |  400.0000 |  400.0000 | 20454450 B |
    |  SystemLinq |   True |     466544 | 297,644,242.1 ns | 6,289,350.972 ns | 6,990,596.941 ns |  1.00 |    0.00 | 11000.0000 | 4000.0000 | 1000.0000 | 61639824 B |
    | CisternLinq |   True |     466544 | 260,308,023.3 ns | 4,252,457.531 ns | 3,977,751.413 ns |  0.87 |    0.03 |  9000.0000 | 3500.0000 |  500.0000 | 54175372 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class String_JoinPalindrome : StringsBenchmarkBase
    {
        private string ReverseString(string s)
        {
            ReadOnlySpan<char> forward = s;
            Span<char> reversed = stackalloc char[forward.Length];

            for (var i = 0; i < forward.Length; ++i)
                reversed[s.Length - i - 1] = forward[i];

            return new string(reversed);
        }

        [Benchmark]
        public int ForLoop()
        {
            var count = 0;

            var words = new HashSet<string>(Words);
            foreach (var forward in Words)
            {
                var reversed = ReverseString(forward);
                if (words.Contains(reversed))
                    ++count;
            }

            return count;
        }

        [Benchmark(Baseline = true)]
        public int SystemLinq()
        {
            return
                System.Linq.Enumerable.Count(
                    System.Linq.Enumerable.Join(Words, Words, x => x, ReverseString, (forward, revered) => (forward, revered)));
        }

        [Benchmark]
        public int CisternLinq()
        {
            return
                Words
                .Join(Words, x=>x, ReverseString, (forward,revered)=>(forward,revered))
                .Count();
        }
    }
}
