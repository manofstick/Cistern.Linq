using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |------- |----------- |----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |  False |         10 |        787.8 ns |         4.135 ns |         3.868 ns |  0.45 |    0.00 |    0.5941 |         - |        - |     1.82 KB |
    |  SystemLinq |  False |         10 |      1,749.8 ns |         4.598 ns |         4.076 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |  False |         10 |      1,522.2 ns |         5.048 ns |         4.475 ns |  0.87 |    0.00 |    0.7915 |         - |        - |     2.43 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  False |       1000 |     86,249.2 ns |       249.489 ns |       221.165 ns |  0.49 |    0.00 |   34.4238 |   11.4746 |        - |   105.76 KB |
    |  SystemLinq |  False |       1000 |    176,647.2 ns |       957.163 ns |       895.330 ns |  1.00 |    0.00 |   50.7813 |   15.6250 |        - |   176.24 KB |
    | CisternLinq |  False |       1000 |    141,977.8 ns |       699.311 ns |       619.921 ns |  0.80 |    0.01 |   43.4570 |    8.5449 |        - |   137.95 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  False |     466544 | 76,755,152.4 ns |   372,846.152 ns |   291,093.751 ns |  0.80 |    0.01 | 1857.1429 |  714.2857 |        - |  11398.8 KB |
    |  SystemLinq |  False |     466544 | 95,261,291.1 ns | 1,348,979.044 ns | 1,261,835.834 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.79 KB |
    | CisternLinq |  False |     466544 | 67,960,996.7 ns |   869,308.037 ns |   813,151.277 ns |  0.71 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11647.88 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |         10 |        720.4 ns |         3.226 ns |         2.859 ns |  0.48 |    0.00 |    0.5379 |         - |        - |     1.65 KB |
    |  SystemLinq |   True |         10 |      1,503.8 ns |         5.043 ns |         4.717 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |   True |         10 |      1,321.3 ns |         8.113 ns |         6.775 ns |  0.88 |    0.01 |    0.7057 |         - |        - |     2.16 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |       1000 |     47,516.5 ns |        96.855 ns |        85.859 ns |  0.71 |    0.00 |   13.5498 |         - |        - |     41.7 KB |
    |  SystemLinq |   True |       1000 |     66,651.3 ns |       262.302 ns |       232.524 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |   True |       1000 |     52,890.1 ns |       284.420 ns |       266.047 ns |  0.79 |    0.01 |   14.0991 |         - |        - |    43.38 KB |
    |             |        |            |                 |                  |                  |       |         |           |           |          |             |
    |     ForLoop |   True |     466544 | 38,200,997.8 ns |   760,319.257 ns |   961,560.416 ns |  0.67 |    0.02 | 2142.8571 |  928.5714 | 285.7143 | 11399.64 KB |
    |  SystemLinq |   True |     466544 | 57,314,390.6 ns | 1,123,615.429 ns | 1,153,870.320 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.68 KB |
    | CisternLinq |   True |     466544 | 41,033,645.1 ns |   423,735.496 ns |   375,630.481 ns |  0.72 |    0.02 | 2000.0000 |  846.1538 | 230.7692 | 10730.29 KB |
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
