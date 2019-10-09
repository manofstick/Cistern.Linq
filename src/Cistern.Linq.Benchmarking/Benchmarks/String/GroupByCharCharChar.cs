using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |            Error |         StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |------- |----------- |----------------:|-----------------:|---------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |  False |         10 |        818.8 ns |         2.723 ns |       2.414 ns |  0.46 |    0.00 |    0.5941 |         - |        - |     1.82 KB |
    |  SystemLinq |  False |         10 |      1,780.3 ns |         4.247 ns |       3.973 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |  False |         10 |      1,771.8 ns |        11.844 ns |       9.891 ns |  1.00 |    0.01 |    0.7439 |         - |        - |     2.28 KB |
    |             |        |            |                 |                  |                |       |         |           |           |          |             |
    |     ForLoop |  False |       1000 |     85,857.9 ns |       357.417 ns |     334.328 ns |  0.48 |    0.00 |   34.4238 |   11.4746 |        - |   105.76 KB |
    |  SystemLinq |  False |       1000 |    178,564.7 ns |     1,634.267 ns |   1,448.735 ns |  1.00 |    0.00 |   50.7813 |   14.8926 |        - |   176.24 KB |
    | CisternLinq |  False |       1000 |    160,734.7 ns |       941.522 ns |     834.635 ns |  0.90 |    0.01 |   40.2832 |   11.4746 |        - |    137.8 KB |
    |             |        |            |                 |                  |                |       |         |           |           |          |             |
    |     ForLoop |  False |     466544 | 76,564,197.8 ns |   424,376.493 ns | 354,373.592 ns |  0.81 |    0.01 | 1857.1429 |  714.2857 |        - |  11398.8 KB |
    |  SystemLinq |  False |     466544 | 94,518,676.7 ns | 1,042,907.001 ns | 975,535.856 ns |  1.00 |    0.00 | 3000.0000 | 1333.3333 | 333.3333 | 16541.23 KB |
    | CisternLinq |  False |     466544 | 66,138,650.0 ns |   852,775.762 ns | 797,686.976 ns |  0.70 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11647.58 KB |
    |             |        |            |                 |                  |                |       |         |           |           |          |             |
    |     ForLoop |   True |         10 |        694.5 ns |         1.956 ns |       1.734 ns |  0.47 |    0.00 |    0.5379 |         - |        - |     1.65 KB |
    |  SystemLinq |   True |         10 |      1,490.8 ns |         3.173 ns |       2.813 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |   True |         10 |      1,522.9 ns |         2.744 ns |       2.567 ns |  1.02 |    0.00 |    0.6580 |         - |        - |     2.02 KB |
    |             |        |            |                 |                  |                |       |         |           |           |          |             |
    |     ForLoop |   True |       1000 |     47,703.5 ns |        81.729 ns |      72.451 ns |  0.72 |    0.00 |   13.5498 |         - |        - |     41.7 KB |
    |  SystemLinq |   True |       1000 |     66,237.7 ns |       188.583 ns |     176.401 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |   True |       1000 |     53,722.1 ns |       130.512 ns |     115.696 ns |  0.81 |    0.00 |   14.0991 |         - |        - |    43.23 KB |
    |             |        |            |                 |                  |                |       |         |           |           |          |             |
    |     ForLoop |   True |     466544 | 37,439,106.8 ns |   716,072.973 ns | 852,433.937 ns |  0.66 |    0.02 | 2142.8571 |  928.5714 | 285.7143 | 11399.65 KB |
    |  SystemLinq |   True |     466544 | 56,865,754.0 ns |   611,450.666 ns | 571,951.333 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.68 KB |
    | CisternLinq |   True |     466544 | 40,525,788.5 ns |   644,487.647 ns | 571,321.512 ns |  0.71 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.04 KB |
    */

    [CoreJob, MemoryDiagnoser]
    public class GroupByCharCharChar : StringsBenchmarkBase
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
