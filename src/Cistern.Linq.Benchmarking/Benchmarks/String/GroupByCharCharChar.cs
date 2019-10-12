using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |------- |----------- |----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |  False |         10 |        822.2 ns |         1.938 ns |         1.619 ns |  0.46 |    0.00 |    0.5941 |         - |        - |     1.82 KB |
    |  SystemLinq |  False |         10 |      1,768.8 ns |         4.936 ns |         4.617 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |  False |         10 |      2,258.9 ns |         4.061 ns |         3.600 ns |  1.28 |    0.00 |    0.9460 |         - |        - |      2.9 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  False |       1000 |     87,124.4 ns |       489.331 ns |       408.614 ns |  0.48 |    0.01 |   34.4238 |   11.4746 |        - |   105.76 KB |
    |  SystemLinq |  False |       1000 |    181,956.2 ns |     2,701.337 ns |     2,255.739 ns |  1.00 |    0.00 |   51.5137 |   14.1602 |        - |   176.24 KB |
    | CisternLinq |  False |       1000 |    187,736.6 ns |     1,241.894 ns |     1,100.907 ns |  1.03 |    0.02 |   54.4434 |   18.0664 |        - |    167.2 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  False |     466544 | 77,399,362.6 ns | 1,707,780.257 ns | 2,032,990.913 ns |  0.81 |    0.03 | 1857.1429 |  714.2857 |        - |  11398.8 KB |
    |  SystemLinq |  False |     466544 | 95,687,881.1 ns | 1,459,219.649 ns | 1,364,954.965 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16542.21 KB |
    | CisternLinq |  False |     466544 | 73,048,950.5 ns | 1,162,889.998 ns | 1,087,768.026 ns |  0.76 |    0.02 | 2428.5714 | 1142.8571 | 428.5714 | 12132.75 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |         10 |        683.8 ns |         3.420 ns |         2.856 ns |  0.45 |    0.00 |    0.5379 |         - |        - |     1.65 KB |
    |  SystemLinq |   True |         10 |      1,504.6 ns |         3.770 ns |         3.526 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |   True |         10 |      1,925.4 ns |        13.400 ns |        12.534 ns |  1.28 |    0.01 |    0.8278 |         - |        - |     2.54 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |       1000 |     48,244.9 ns |       352.789 ns |       312.738 ns |  0.72 |    0.00 |   13.5498 |         - |        - |     41.7 KB |
    |  SystemLinq |   True |       1000 |     66,930.6 ns |       154.575 ns |       144.590 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |   True |       1000 |     64,702.5 ns |       218.453 ns |       193.652 ns |  0.97 |    0.00 |   15.6250 |         - |        - |    48.13 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |     466544 | 38,457,232.0 ns |   758,120.797 ns |   842,649.257 ns |  0.69 |    0.03 | 2142.8571 |  928.5714 | 285.7143 | 11399.56 KB |
    |  SystemLinq |   True |     466544 | 55,560,037.5 ns | 1,108,739.424 ns | 1,088,930.366 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.59 KB |
    | CisternLinq |   True |     466544 | 45,986,078.2 ns |   682,379.434 ns |   638,298.146 ns |  0.83 |    0.02 | 2090.9091 |  909.0909 | 272.7273 | 11215.59 KB |
    */

    [CoreJob, MemoryDiagnoser]
    public class String_GroupByCharCharChar : StringsBenchmarkBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var answer = new DesiredShape();

            foreach (var n in Words)
            {
                if (n.Length >= 3)
                {
                    var key = (n[0], n[1], n[2]);
                    if (!answer.TryGetValue(key, out var words))
                    {
                        words = new List<string>();
                        answer[key] = words;
                    }
                    words.Add(n);
                }
            }

            return answer;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return
                System.Linq.Enumerable.ToDictionary(
                    System.Linq.Enumerable.GroupBy(
                        System.Linq.Enumerable.Where(Words, w => w.Length >= 3),
                            w => (w[0], w[1], w[2])),
                                ws => ws.Key,
                                ws => System.Linq.Enumerable.ToList(ws));
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Words
                .Where(w => w.Length >= 3)
                .GroupBy(w => (w[0], w[1], w[2]))
                .ToDictionary(ws => ws.Key, ws => ws.ToList());
        }
    }
}
