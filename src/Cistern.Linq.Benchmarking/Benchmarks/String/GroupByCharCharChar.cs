using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |---------:|----------:|----------:|------:|--------:|----------:|----------:|---------:|----------:|
    |     ForLoop |  False |         10 | 78.61 ms | 1.5245 ms | 1.7556 ms |  0.81 |    0.02 | 1857.1429 |  714.2857 |        - |  11.13 MB |
    |  SystemLinq |  False |         10 | 96.32 ms | 0.6835 ms | 0.6394 ms |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 |  16.15 MB |
    | CisternLinq |  False |         10 | 85.17 ms | 0.4639 ms | 0.4340 ms |  0.88 |    0.00 | 2142.8571 | 1000.0000 | 285.7143 |  11.37 MB |
    |             |        |            |          |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |       1000 | 77.11 ms | 1.5734 ms | 1.6158 ms |  0.81 |    0.02 | 1857.1429 |  714.2857 |        - |  11.13 MB |
    |  SystemLinq |  False |       1000 | 95.79 ms | 1.0531 ms | 0.9851 ms |  1.00 |    0.00 | 3000.0000 | 1333.3333 | 333.3333 |  16.15 MB |
    | CisternLinq |  False |       1000 | 84.99 ms | 0.6243 ms | 0.5534 ms |  0.89 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 |  11.38 MB |
    |             |        |            |          |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |     466544 | 77.14 ms | 0.4111 ms | 0.3433 ms |  0.81 |    0.01 | 1857.1429 |  714.2857 |        - |  11.13 MB |
    |  SystemLinq |  False |     466544 | 95.51 ms | 1.6955 ms | 1.5860 ms |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 |  16.15 MB |
    | CisternLinq |  False |     466544 | 85.08 ms | 0.3560 ms | 0.3156 ms |  0.89 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 |  11.37 MB |
    |             |        |            |          |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |         10 | 38.33 ms | 0.7349 ms | 0.8168 ms |  0.69 |    0.02 | 2142.8571 |  928.5714 | 285.7143 |  11.13 MB |
    |  SystemLinq |   True |         10 | 55.17 ms | 1.0492 ms | 1.2490 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  16.15 MB |
    | CisternLinq |   True |         10 | 40.11 ms | 0.5261 ms | 0.4921 ms |  0.73 |    0.02 | 2000.0000 |  846.1538 | 230.7692 |  10.48 MB |
    |             |        |            |          |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |       1000 | 39.42 ms | 0.7696 ms | 0.7558 ms |  0.69 |    0.01 | 2142.8571 |  928.5714 | 285.7143 |  11.13 MB |
    |  SystemLinq |   True |       1000 | 56.99 ms | 0.3749 ms | 0.3131 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  16.15 MB |
    | CisternLinq |   True |       1000 | 41.63 ms | 0.7247 ms | 0.6778 ms |  0.73 |    0.01 | 2000.0000 |  846.1538 | 230.7692 |  10.48 MB |
    |             |        |            |          |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |     466544 | 39.75 ms | 0.7266 ms | 0.6797 ms |  0.70 |    0.02 | 2142.8571 |  928.5714 | 285.7143 |  11.13 MB |
    |  SystemLinq |   True |     466544 | 57.05 ms | 1.1445 ms | 1.0706 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  16.15 MB |
    | CisternLinq |   True |     466544 | 40.90 ms | 0.2620 ms | 0.2046 ms |  0.72 |    0.01 | 2000.0000 |  846.1538 | 230.7692 |  10.48 MB |
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
