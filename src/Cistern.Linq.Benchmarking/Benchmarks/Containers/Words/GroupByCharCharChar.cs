using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method | Sorted | WordsCount |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|----------:|---------:|----------:|
    |     ForLoop |  False |         10 |  89.39 ms | 0.6280 ms | 0.5244 ms |  0.84 |    0.01 | 2000.0000 |  833.3333 | 166.6667 |  11.92 MB |
    |  SystemLinq |  False |         10 | 106.22 ms | 0.7228 ms | 0.6761 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.06 MB |
    | CisternLinq |  False |         10 |  95.43 ms | 0.9036 ms | 0.8452 ms |  0.90 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |       1000 |  89.10 ms | 0.8637 ms | 0.8079 ms |  0.84 |    0.01 | 1833.3333 |  833.3333 |        - |  11.92 MB |
    |  SystemLinq |  False |       1000 | 106.27 ms | 0.7929 ms | 0.7029 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.07 MB |
    | CisternLinq |  False |       1000 |  95.94 ms | 0.9415 ms | 0.8807 ms |  0.90 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |     466544 |  89.37 ms | 0.8361 ms | 0.6528 ms |  0.85 |    0.01 | 1833.3333 |  833.3333 |        - |  11.92 MB |
    |  SystemLinq |  False |     466544 | 105.27 ms | 0.8249 ms | 0.7312 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.07 MB |
    | CisternLinq |  False |     466544 |  95.56 ms | 0.7231 ms | 0.6764 ms |  0.91 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |         10 |  38.47 ms | 0.3511 ms | 0.3284 ms |  0.66 |    0.01 | 2071.4286 |  857.1429 | 214.2857 |  11.92 MB |
    |  SystemLinq |   True |         10 |  58.66 ms | 0.3928 ms | 0.3674 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  17.06 MB |
    | CisternLinq |   True |         10 |  46.68 ms | 0.3610 ms | 0.3200 ms |  0.80 |    0.01 | 2000.0000 |  818.1818 | 181.8182 |  11.55 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |       1000 |  38.45 ms | 0.4311 ms | 0.4033 ms |  0.66 |    0.02 | 2076.9231 |  846.1538 | 230.7692 |  11.92 MB |
    |  SystemLinq |   True |       1000 |  58.26 ms | 1.1258 ms | 1.1057 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  17.07 MB |
    | CisternLinq |   True |       1000 |  45.29 ms | 0.4813 ms | 0.4502 ms |  0.78 |    0.02 | 2090.9091 |  909.0909 | 272.7273 |  11.55 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |     466544 |  38.66 ms | 0.3569 ms | 0.3339 ms |  0.66 |    0.01 | 2071.4286 |  857.1429 | 214.2857 |  11.92 MB |
    |  SystemLinq |   True |     466544 |  58.44 ms | 0.5320 ms | 0.4976 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  17.06 MB |
    | CisternLinq |   True |     466544 |  46.73 ms | 0.4770 ms | 0.4462 ms |  0.80 |    0.01 | 2000.0000 |  818.1818 | 181.8182 |  11.55 MB |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_GroupByCharCharChar : WordsBase
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
